using ICE.GDocs.Common.Core.Domain.ValueObjects;
using ICE.GDocs.Domain.Models.Trace;
using ICE.GDocs.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.Handlers
{
    public class RequestResponseTracingHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public RequestResponseTracingHandlerMiddleware(
            RequestDelegate next,
            IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var newScope = _serviceProvider.CreateScope();
            var configuration = newScope.ServiceProvider.GetRequiredService<IConfiguration>();
            newScope.Dispose();

            var urlsNaoRastreaveis = configuration.GetSection("Rastreabilidade:UrlsNaoRastreaveis")?.Get<string[]>()
                      ?? Array.Empty<string>();
            if (urlsNaoRastreaveis.ToList().Exists(url => request.Path.Value.ToLowerInvariant().Contains(url.ToLowerInvariant())))
            {
                await _next(httpContext);
                return;
            }

            var trace = new TraceModel();
            var traceInalterada = new TraceModel();
            var stopWatch = Stopwatch.StartNew();
            var originalBodyStream = httpContext.Response.Body;

            trace.Requisicao = await CriarTraceRequisicao(request);
            traceInalterada.Requisicao = await CriarTraceRequisicao(request);

            trace.Requisicao.Body = RemoverCamposNaoRastreaveis(trace.Requisicao.Body, configuration);

            using (var responseBody = new MemoryStream())
            {
                var response = httpContext.Response;
                response.Body = responseBody;

                var status = string.Empty;
                var responseStatus = string.Empty;

                try
                {
                    await _next(httpContext);

                    responseStatus = "Completed";
                    status = $"{responseStatus}:{response.StatusCode.ToEnum<HttpStatusCode>().GetDescription()}";

                    trace.Resposta = await CriarTraceResposta(response, responseStatus);
                    traceInalterada.Resposta = await CriarTraceResposta(response, responseStatus);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    responseStatus = "Error";
                    status = $"{responseStatus}:{ex.Message}";

                    throw;
                }
                finally
                {
                    stopWatch.Stop();

                    var metadados = Metadados.Criar(ConstruirMetadados(trace, httpContext, status, stopWatch.Elapsed));

                    var conteudo = MetadadosTrace.Criar(trace);

                    var rastreabilidade = DadosRequisicao.Criar(request.Path, metadados, conteudo);

                    RegistrarRastreabilidadeComResilienciaSemAguardar(rastreabilidade);
                }
            }

        }

        private IDictionary<string, object> ConstruirMetadados(TraceModel trace, HttpContext httpContext, string status, TimeSpan elapsed)
        {
            IDictionary<string, object> metadata = new ExpandoObject();

            if (!string.IsNullOrWhiteSpace(trace.Requisicao.Body))
            {
                try
                {
                    metadata = JsonConvert.DeserializeObject<IDictionary<string, object>>(trace.Requisicao.Body);
                }
                catch
                {
                    metadata.Add("root", trace.Requisicao.Body);
                }
            }

            httpContext.GetRouteData()?.Values.ForEach(parameters =>
            {
                metadata.Add(parameters.Key, parameters.Value);
            });


            metadata.Add("Origem", "GDocs.Api");

            metadata.Add("Recurso", httpContext.Request.Path.Value);
            metadata.Add("Status", status);
            metadata.Add("Tempo", elapsed);

            return metadata;
        }

        private string RemoverCamposNaoRastreaveis(string requestBody, IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(requestBody))
                return "";

            var camposNaoRastreaveisMetadados = configuration.GetSection("Rastreabilidade:CamposNaoRastreaveisMetadados")?.Get<string[]>()
             ?? Array.Empty<string>();

            try
            {
                var metadata = JsonConvert.DeserializeObject<IDictionary<string, object>>(requestBody);

                camposNaoRastreaveisMetadados.ForEach(campo =>
                {
                    if (metadata.ContainsKey(campo))
                        metadata.Remove(campo);
                });

                return JsonConvert.SerializeObject(metadata);
            }
            catch
            {
                try
                {
                    var metadataList = JsonConvert.DeserializeObject<IList<IDictionary<string, object>>>(requestBody);

                    camposNaoRastreaveisMetadados.ForEach(campo =>
                    {
                        metadataList.ForEach(metadata =>
                        {
                            if (metadata.ContainsKey(campo))
                                metadata.Remove(campo);
                        });
                    });

                    return JsonConvert.SerializeObject(metadataList);
                }
                catch
                {
                    return requestBody;
                }
            }
        }

        private async Task<TraceRequisicaoModel> CriarTraceRequisicao(HttpRequest httpRequest)
        {
            var requestBodyContent = await ReadRequestBody(httpRequest);

            return new TraceRequisicaoModel(httpRequest.Headers.AsList())
            {
                BaseUrl = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}",
                Method = httpRequest.Method,
                Resource = httpRequest.Path,
                QueryString = httpRequest.QueryString.ToString(),
                Body = requestBodyContent
            };
        }

        private async Task<TraceRespostaModel> CriarTraceResposta(HttpResponse httpResponse, string responseStatus)
        {
            var responseBodyContent = await ReadResponseBody(httpResponse);

            return new TraceRespostaModel(httpResponse.Headers.AsList())
            {
                Content = responseBodyContent,
                ContentType = httpResponse.ContentType,
                ContentLength = httpResponse.ContentLength.GetValueOrDefault(responseBodyContent.Length),
                StatusCode = httpResponse.StatusCode.ToEnum<HttpStatusCode>(),
                ResponseStatus = responseStatus
            };
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            string bodyAsText;

            var mem = new MemoryStream();
            try
            {
                using (var reader = new StreamReader(mem))
                {
                    await request.Body.CopyToAsync(mem);
                    await reader.ReadToEndAsync();
                    mem.Seek(0, SeekOrigin.Begin);
                    bodyAsText = await reader.ReadToEndAsync();
                }

                request.Body.Seek(0, SeekOrigin.Begin);

                mem = null;

                return bodyAsText;
            }
            finally
            {
                if (mem != null)
                    mem.Dispose();
            }

        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private void RegistrarRastreabilidadeComResilienciaSemAguardar(DadosRequisicao dadosRequisicao)
        {
            Task.Run(async () =>
            {
                using (var newScope = _serviceProvider.CreateScope())
                {
                    var configuration = newScope.ServiceProvider.GetRequiredService<IConfiguration>();
                    var logger = newScope.ServiceProvider.GetRequiredService<ILogger<RequestResponseTracingHandlerMiddleware>>();
                    var applicationLifetime = newScope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IApplicationLifetime>();
                    var rastreabilidadeRepository = newScope.ServiceProvider.GetRequiredService<ILogRepository>();

                    var retryIntervals = configuration.GetSection("Rastreabilidade:Retentativas:Intervalos")?.Get<TimeSpan[]>()
                        ?? Array.Empty<TimeSpan>();

                    var resultadoComPolicy =
                        await Policy
                            .Handle<Exception>()
                            .WaitAndRetryAsync(
                                retryIntervals,
                                (exception, tempoProximaTentativa, tentativa, _) =>
                                {
                                    if (exception is var falha)
                                        logger.LogWarning(
                                            falha,
                                            $"Policy.WaitAndRetryAsync - Falha ao registrar a rastreabilidade. Tentativa: {tentativa}, Proxima tentativa em: {tempoProximaTentativa}, Metadados: {dadosRequisicao.Metadados?.DadosXml?.InnerXml}."
                                        );
                                }
                            )
                            .ExecuteAndCaptureAsync(async (_) =>
                            {
                                await rastreabilidadeRepository.InserirDadosRequisicao(dadosRequisicao);

                                return Task.CompletedTask;
                            }
                            , applicationLifetime.ApplicationStopping);

                    if (resultadoComPolicy.Outcome == OutcomeType.Successful)
                        return;

                    var errorMessage = $"Erro ao registrar a rastreabilidade. Metadados: {dadosRequisicao.Metadados?.DadosXml?.InnerXml}.";

                    if (resultadoComPolicy.FinalException is var error)
                    {
                        logger.LogError(error, errorMessage);
                        return;
                    }

                    logger.LogError(errorMessage);
                }
            }).ConfigureAwait(false).GetAwaiter();
        }
    }
}
