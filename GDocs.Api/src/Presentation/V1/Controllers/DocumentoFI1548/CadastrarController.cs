using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI1548/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class CadastrarController : ControllerBase
    {        
        private readonly ILogService _logService;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;
        private readonly INodeServices _nodeServices;
        private readonly IConfiguration _configuration;
        private readonly ISequencialService _sequencialService;
        private readonly IDocToolsAppService _docToolsAppService;

        public CadastrarController(            
            ILogService logService,
            IDocumentoFI1548AppService documentoFI1548AppService,
            INodeServices nodeServices,
            IConfiguration configuration,
            ISequencialService sequencialService,
            IDocToolsAppService docToolsAppService
        )
        {            
            _logService = logService;
            _documentoFI1548AppService = documentoFI1548AppService;
            _nodeServices = nodeServices;
            _configuration = configuration;
            _sequencialService = sequencialService;
            _docToolsAppService = docToolsAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<int>> Post(
            DocumentoFI1548Model documento,
            CancellationToken cancellationToken = default
        )
        {
            await _logService.AdicionarRastreabilidade(documento, "Gerando base64 do pdf do documento.");

            if (!documento.PodeEditar())
            {

                var sequencial = await _sequencialService.ObterProximoSequencialStatusPagamento();
                if (sequencial.IsFailure)
                    return this.Failure(sequencial.Failure);

                documento.DefinirNumero(sequencial.Success);
            }

            var usuarioLogado = this.ObterUsuario();

            var pdfBase64 = await GerarPdfBase64FI1548(documento, usuarioLogado, cancellationToken);
            if (pdfBase64.IsFailure)
                return this.Failure(pdfBase64.Failure);

            await _logService.AdicionarRastreabilidade(documento, "base64 pdf retornado.");
            
            documento.ArquivoBinario = pdfBase64.Success;

            var response = await _documentoFI1548AppService.Adicionar(documento, usuarioLogado, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);
        }

        private async Task<TryException<byte[]>> GerarPdfBase64FI1548(
            DocumentoFI1548Model documento,
            UsuarioModel usuarioModel,
            CancellationToken cancellationToken
        )
        {
            var xlsByteArray = await ConverterDocumentoFI1548ToXlsByteArray(documento, usuarioModel);
            return await ConverterXlsByteArrayToPdfBase64(xlsByteArray, cancellationToken);
        }

        private string VerificarParametroMarcadoNoDocumento(string valor, string valorDeComparacao)
            => valor.ToLowerInvariant() == valorDeComparacao.ToLowerInvariant() ? "X" : string.Empty;

        private async Task<byte[]> ConverterDocumentoFI1548ToXlsByteArray(
            DocumentoFI1548Model documento,
            UsuarioModel usuarioModel
        )
        {
            var pathTemplateDocument = _configuration.GetValue("DocumentoFI1548:PathTemplate", string.Empty);
            var bytesTemplateDocument = System.IO.File.ReadAllBytes(pathTemplateDocument);
            var byteTemplateDocumentString = BitConverter.ToString(bytesTemplateDocument);

            var xlsBase64 = await _nodeServices.InvokeAsync<string>("./Nodejs/xlsxTransform", byteTemplateDocumentString, new
            {
                numero = documento.Numero,
                referencia = documento.Referencia,
                descricao = documento.Descricao,
                pagamento = new
                {                    
                    valor = documento.Valor.ToString("N", new CultureInfo("pt-BR")),
                    unico = VerificarParametroMarcadoNoDocumento(documento.TipoPagamento, "unico"),
                    parcial = VerificarParametroMarcadoNoDocumento(documento.TipoPagamento, "parcial"),
                    parcelado = VerificarParametroMarcadoNoDocumento(documento.TipoPagamento, "parcelado"),
                    vezes = documento.QuantidadeParcelas > 0 ? documento.QuantidadeParcelas.ToString() : "",
                    moedaSimbolo = documento.MoedaSimbolo
                },
                fornecedor = new
                {
                    unico = VerificarParametroMarcadoNoDocumento(documento.Fornecedor, "unico"),
                    diversos = VerificarParametroMarcadoNoDocumento(documento.Fornecedor, "diversos")
                },
                prazoEntrega = documento.PrazoEntrega,
                vencimentoDias = documento.VencimentoPara > 0 ? documento.VencimentoPara.ToString() : "",
                solicitante = usuarioModel.Nome,
                data = DateTime.Now.ToString("dd/MM/yyyy")
            });

            return Convert.FromBase64String(xlsBase64);
        }

        private async Task<TryException<byte[]>> ConverterXlsByteArrayToPdfBase64(byte[] xlsByteArray, CancellationToken cancellationToken)
        {
            var arquivoPdf = await _docToolsAppService.Converter(".pdf", new ArquivoModel { Binario = xlsByteArray, Extensao = ".xlsx" }, cancellationToken);

            if (arquivoPdf.IsFailure)
                return arquivoPdf.Failure;

            return arquivoPdf.Success.Binario;
        }
    }
}
