using ICE.GDocs.Api.Security;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.Assinatura
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Assinatura/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class GerenciamentoRdlBytesConverterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;
        private readonly IRdlToPdfBytesConverterExternalService _rdlToPdfBytesConverterExternalService;

        public GerenciamentoRdlBytesConverterController(
            IConfiguration configuration,
            IAuthorizationService authService,
            IGDocsCacheExternalService gDocsCacheExternalService,
            IRdlToPdfBytesConverterExternalService rdlToPdfBytesConverterExternalService
            )
        {
            _configuration = configuration;
            _authService = authService;
            _gDocsCacheExternalService = gDocsCacheExternalService;
            _rdlToPdfBytesConverterExternalService = rdlToPdfBytesConverterExternalService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<byte[]>> PostAsync(
            AssinaturaInformacoesFilterModel filtro,
            CancellationToken cancellationToken = default
        )
        {
            filtro.UsuariosGuidAd = await this.HasRoleAsync(_authService, "assinatura:gerenciardocumentos:todosusuarios")
                ? filtro.UsuariosGuidAd : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };

            var construirParametrosParaReport = await ConstruirParametrosParaReport(filtro);
            if (construirParametrosParaReport.IsFailure)
                return this.Failure(construirParametrosParaReport.Failure);

            var response = _rdlToPdfBytesConverterExternalService.Converter(
                _configuration.GetValue("GerenciamentoAssinatura:RdlPath", string.Empty),
                construirParametrosParaReport.Success
            );

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);

        }

        private async Task<TryException<Dictionary<string, string>>> ConstruirParametrosParaReport(AssinaturaInformacoesFilterModel assinaturaInformacoesFilterModel)
        {
            var adicionarFiltrosGerenciamentoAssinaturaParaReportServer = await _gDocsCacheExternalService.AdicionarFiltrosGerenciamentoAssinaturaParaReportServer(assinaturaInformacoesFilterModel, TimeSpan.FromMinutes(1));
            if (adicionarFiltrosGerenciamentoAssinaturaParaReportServer.IsFailure)
                return adicionarFiltrosGerenciamentoAssinaturaParaReportServer.Failure;

            var parametros = new Dictionary<string, string> {
                {
                    "xmlConnectString",
                    $"{Request.Scheme}://{Request.Host}/api/v1/reportservertools/obtergerenciamentoassinatura/{adicionarFiltrosGerenciamentoAssinaturaParaReportServer.Success}.xml"
                }
            };

            if (assinaturaInformacoesFilterModel.DataInicio != null)
            {
                parametros.Add("dataInicio", $"{assinaturaInformacoesFilterModel.DataInicio:dd/MM/yyyy}");
            }

            if (assinaturaInformacoesFilterModel.DataTermino != null)
            {
                parametros.Add("dataTermino", $"{assinaturaInformacoesFilterModel.DataTermino:dd/MM/yyyy}");
            }

            return parametros;
        }
    }
}
