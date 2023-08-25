using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.ExternalServices.DocTools
{
    internal partial class DocToolsExternalService : IDocToolsExternalService
    {
        private readonly IConfiguration _configuration;

        public DocToolsExternalService
        (
            IConfiguration configuration

        )
        {
            _configuration = configuration;
        }

        public async Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken)
        {
            var baseUrl = _configuration.GetValue("Infra:ExternalServices:DocToolsApi:BaseUrl", string.Empty);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"{baseUrl}/v1/converter/para/{extensao}");
            httpWebRequest.UseDefaultCredentials = true;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("access-control-allow-origin", "*");
            httpWebRequest.Headers.Add("access-control-allow-credentials", "true");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(arquivo));
            }

            var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<ArquivoModel>(streamReader.ReadToEnd());
            }
        }

    }
}
