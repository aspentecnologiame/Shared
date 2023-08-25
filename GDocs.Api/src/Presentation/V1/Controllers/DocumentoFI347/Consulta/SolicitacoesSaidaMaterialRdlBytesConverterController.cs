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
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI347.Consulta
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class SolicitacoesSaidaMaterialRdlBytesConverterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;
        private readonly IRdlToPdfBytesConverterExternalService _rdlToPdfBytesConverterExternalService;

        public SolicitacoesSaidaMaterialRdlBytesConverterController(
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

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<byte[]>> PostAsync(
            SolicitacaoSaidaMaterialFilterModel filtro
        )
        {
            filtro.Autores = await this.HasRoleAsync(_authService, "fi347:consultar:todosusuarios")
                ? filtro.Autores : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };

            filtro.UsuarioLogadoAd = this.ObterUsuario().ActiveDirectoryId;

            var construirParametrosParaReport = await ConstruirParametrosParaReport(filtro);
            if (construirParametrosParaReport.IsFailure)
                return this.Failure(construirParametrosParaReport.Failure);

            var response = _rdlToPdfBytesConverterExternalService.Converter(
                _configuration.GetValue("DocumentoFI347:SolicitacoesSaidaMaterial:RdlPath", string.Empty),
                construirParametrosParaReport.Success
            );

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);

        }

        private async Task<TryException<Dictionary<string, string>>> ConstruirParametrosParaReport(SolicitacaoSaidaMaterialFilterModel solicitacaoSaidaMaterialFilterModel)
        {
            var adicionarFiltrosSolicitacoesSaidaMaterialParaReportServer = await _gDocsCacheExternalService.AdicionarFiltrosSolicitacoesSaidaMaterialParaReportServer(solicitacaoSaidaMaterialFilterModel, TimeSpan.FromMinutes(1));
            if (adicionarFiltrosSolicitacoesSaidaMaterialParaReportServer.IsFailure)
                return adicionarFiltrosSolicitacoesSaidaMaterialParaReportServer.Failure;

            var parametros = new Dictionary<string, string> {
                {
                    "xmlConnectString",
                    $"{Request.Scheme}://{Request.Host}/api/v1/reportservertools/obtersolicitacoessaidamaterial/{adicionarFiltrosSolicitacoesSaidaMaterialParaReportServer.Success}.xml"
                }
            };

            if (solicitacaoSaidaMaterialFilterModel.DataInicio != null)
            {
                parametros.Add("dataInicio", $"{solicitacaoSaidaMaterialFilterModel.DataInicio:dd/MM/yyyy}");
            }

            if (solicitacaoSaidaMaterialFilterModel.DataTermino != null)
            {
                parametros.Add("dataTermino", $"{solicitacaoSaidaMaterialFilterModel.DataTermino:dd/MM/yyyy}");
            }

            return parametros;
        }
    }
}
