using ICE.GDocs.Api.FileTools.Ghostscript;
using ICE.GDocs.Api.Security;
using ICE.GDocs.Application;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class UploadController : ControllerBase
    {
        private readonly string _uploadTempPath;
        private readonly ILogger<UploadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDocToolsAppService _docToolsAppService;
        private readonly IAssinaturaUsuarioService _assinaturaUsuarioService;
        private readonly IConfiguracaoAppService _configuracaoAppService;

        private const string UPLOAD_ARQUIVO_INVALIDO = "Não foi possível acessar as informações do arquivo selecionado. Realize o upload de um novo arquivo.";
        private const string UPLOAD_EXTENSAO_INVALIDA = "Não é permitido o upload do arquivo selecionado. Realize o upload de um arquivo válido.<br>Extensões validas: {0}";
        private const string REQUISICAO_INVALIDA_ASSINATURA = "Não é permitido informar assinatura no documento para a categoria informada.";
        private const string EXTENSAO_CONVERSAO_ARQUIVO = ".pdf";
        private const long CALC_BYTES_PDF_COMPRESS = 1000;

        public UploadController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<UploadController> logger,
            IConfiguration configuration,
            IDocToolsAppService docToolsAppService,
            IAssinaturaUsuarioService assinaturaUsuarioService,
            IConfiguracaoAppService configuracaoAppService
            )
        {
            _logger = logger;
            _configuration = configuration;
            _docToolsAppService = docToolsAppService;
            _assinaturaUsuarioService = assinaturaUsuarioService;
            _configuracaoAppService = configuracaoAppService;

            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            _uploadTempPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", "temp");
        }

        [ApiExplorerSettings(GroupName = "Upload")]
        [AuthorizeBearer(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<UploadModel>> Post(
            IFormFile file,
            [FromForm] RequisicaoUploadModel requisicao,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var pdfCompress = false;

                var configCategoriaInfo = await ObterConfiguracaoDaCategoria(requisicao.CategoriaId, cancellationToken);

                if (configCategoriaInfo.IsFailure)
                    return this.Failure(configCategoriaInfo.Failure);

                var uploadValido = ValidarArquivoUpload(file, requisicao, configCategoriaInfo.Success, ref pdfCompress);

                if (uploadValido.IsFailure)
                    return this.Failure(uploadValido.Failure);

                var requisicaoValida = ValidarRequisicao(requisicao, configCategoriaInfo.Success);

                if (requisicaoValida.IsFailure)
                    return this.Failure(requisicaoValida.Failure);

                GarantirCaminhoArquivoTemporario(_uploadTempPath);

                var fileNameOriginal = file.FileName;
                var fileNameSaved = $"{Guid.NewGuid()}{EXTENSAO_CONVERSAO_ARQUIVO}";

                CustomizarArquivo customArquivo = new CustomizarArquivo(
                    _assinaturaUsuarioService,
                    configCategoriaInfo.Success,
                    _configuration);

                var ajusteDocumento = await customArquivo.AjustarDocumentoComNumeracaoAutomaticaEAssinaturaDocumento(file, requisicao, uploadValido.Success, cancellationToken);

                if (ajusteDocumento.IsFailure)
                    return this.Failure(ajusteDocumento.Failure);

                var arquivoConvertido = await Converter(ajusteDocumento.Success, fileNameOriginal, EXTENSAO_CONVERSAO_ARQUIVO, cancellationToken);

                if (arquivoConvertido.IsFailure)
                    return this.Failure(arquivoConvertido.Failure);

                var filePath = Path.Combine(_uploadTempPath, fileNameSaved);
                var filePathNotCompress = Path.Combine(_uploadTempPath, $"full_{fileNameSaved}");

                var assinaturaRetorno = new UploadModel()
                {
                    NomeOriginal = fileNameOriginal,
                    NomeSalvo = fileNameSaved,
                    NumeracaoAutomatica = requisicao.NumeroDocumento
                };

                var filePathStream = pdfCompress ? filePathNotCompress : filePath;

                using (var streamArquivoConvertido = new MemoryStream(arquivoConvertido.Success.Binario))
                using (var fileStream = new FileStream(filePathStream, FileMode.Create))
                    await streamArquivoConvertido.CopyToAsync(fileStream, cancellationToken);

                if (pdfCompress)
                    CompressPdf(filePathNotCompress, filePath);

                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    assinaturaRetorno.ArquivoBinario = FileStreamToBase64(fileStream);

                return this.Success(assinaturaRetorno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<TryException<ArquivoModel>> Converter(MemoryStream ms, string nomeArquivo, string extensaoParaConverter, CancellationToken cancellationToken)
        {
            var extensaoArquivoOriginal = Path.GetExtension(nomeArquivo);

            var arquivoOriginalBytes = FileStreamToByteArray(ms);

            var arquivoOriginalParaConverter = new ArquivoModel()
            {
                Extensao = extensaoArquivoOriginal,
                Binario = arquivoOriginalBytes
            };

            if (extensaoArquivoOriginal.ToUpperInvariant() == extensaoParaConverter.ToUpperInvariant())
                return arquivoOriginalParaConverter;

            var arquivoConvertido = await _docToolsAppService.Converter(extensaoParaConverter, arquivoOriginalParaConverter, cancellationToken);

            if (arquivoConvertido.IsFailure)
                return arquivoConvertido.Failure;

            return arquivoConvertido.Success;
        }

        private TryException<string> ValidarArquivoUpload(IFormFile arquivo, RequisicaoUploadModel requisicao, ConfiguracaoCategoriaModel configCategoria, ref bool pdfCompress)
        {
            if (arquivo == null || arquivo.Length <= 0)
                return new BusinessException("upload-arquivo-invalido", UPLOAD_ARQUIVO_INVALIDO);

            var extensoesPermitidas = _configuration.GetValue<string>("UploadFile:AllowedExtensions", string.Empty).ToUpperInvariant();

            if (configCategoria != null && configCategoria.ExtensoesPermitidas.Length > 0 && requisicao.QtdeDocumentos == 0)
                extensoesPermitidas = configCategoria.ExtensoesPermitidas.ToUpperInvariant();

            var extensaoArquivoUpload = Path.GetExtension(arquivo.FileName).Replace(".", "").ToUpperInvariant();

            if (!extensoesPermitidas.Split("|").Contains(extensaoArquivoUpload))
                return new BusinessException("upload-extensao-invalida", string.Format(UPLOAD_EXTENSAO_INVALIDA, extensoesPermitidas));

            var fileSizeKbytesForPdfCompress = _configuration.GetValue<long>("UploadFile:FileSizeKbytesForPdfCompress", 0);

            if (arquivo.Length > (fileSizeKbytesForPdfCompress * CALC_BYTES_PDF_COMPRESS))
                pdfCompress = true;

            return extensaoArquivoUpload;
        }

        private TryException<Return> ValidarRequisicao(RequisicaoUploadModel requisicao, ConfiguracaoCategoriaModel configCategoria)
        {
            if (requisicao.ListaGuidUsuarioAssinaturaDocumento == null)
                requisicao.ListaGuidUsuarioAssinaturaDocumento = new List<Guid>();

            if (configCategoria != null &&
                requisicao.ListaGuidUsuarioAssinaturaDocumento.Any() &&
               !configCategoria.AssinaturaDocumento.Habilitado)
                return new BusinessException("requisicao-invalida", REQUISICAO_INVALIDA_ASSINATURA);

            return Return.Empty;
        }

        private static void GarantirCaminhoArquivoTemporario(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void CompressPdf(string filePathNotCompress, string filePath)
        {
            var compress = new CompressPdf(filePathNotCompress.Replace('\"', ' ').Trim(), filePath.Replace('\"', ' ').Trim());
            compress.ProcessFiles();
        }

        private async Task<TryException<ConfiguracaoCategoriaModel>> ObterConfiguracaoDaCategoria(int categoriaId, CancellationToken cancellationToken)
        {
            var configuracaoCategoria = await _configuracaoAppService.ObterConfiguracaoRepositorio(_configuration.GetValue("ChaveConfiguracao:Categoria", "configCategorias"), cancellationToken);
            if (configuracaoCategoria.IsFailure)
                return configuracaoCategoria.Failure;

            var configCategorias = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfiguracaoCategoriaModel>>(configuracaoCategoria.Success.Valor);

            return configCategorias.Find(w => w.Codigo == categoriaId);
        }

        private string FileStreamToBase64(FileStream fileStream)
        {
            byte[] filebytes = new byte[fileStream.Length];
            fileStream.Read(filebytes, 0, Convert.ToInt32(fileStream.Length));
            return Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
        }

        private byte[] FileStreamToByteArray(Stream stream)
        {
            byte[] filebytes = new byte[stream.Length];
            stream.Read(filebytes, 0, Convert.ToInt32(stream.Length));
            return filebytes;
        }
    }
}
