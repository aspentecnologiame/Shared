using ICE.GDocs.Api.Security;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Consulta
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/consulta/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class SaidaMaterialNfRdlBytesConverterController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;
        private readonly IRdlToPdfBytesConverterExternalService _rdlToPdfBytesConverterExternalService;

        public SaidaMaterialNfRdlBytesConverterController(
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

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<byte[]>> PostAsync(
            SaidaMaterialNotaFiscalFilterModel filtro
        )
        {
            filtro.Autores = await this.HasRoleAsync(_authService, "SaidaMaterialNF:consulta:todosusuario")
                ? filtro.Autores : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };


            var construirParametrosParaReport = await ConstruirParametrosParaReport(filtro);
            if (construirParametrosParaReport.IsFailure)
                return this.Failure(construirParametrosParaReport.Failure);

            var response = _rdlToPdfBytesConverterExternalService.Converter(
                _configuration.GetValue("SaidaMaterialNotaFiscal:SolicitacoesSaidaMaterial:RdlPath", string.Empty),
                construirParametrosParaReport.Success
            );

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);
        }

        private async Task<TryException<Dictionary<string, string>>> ConstruirParametrosParaReport(SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFilterModel)
        {
            var adicionarFiltrosSaidaMaterialNFParaReportServer = await _gDocsCacheExternalService.AdicionarFiltrosSaidaMaterialNFParaReportServer(saidaMaterialNotaFiscalFilterModel, TimeSpan.FromMinutes(1));
            if (adicionarFiltrosSaidaMaterialNFParaReportServer.IsFailure)
                return adicionarFiltrosSaidaMaterialNFParaReportServer.Failure;

            var parametros = new Dictionary<string, string> {
                {
                    "xmlConnectString",
                    $"{Request.Scheme}://{Request.Host}/api/v1/reportservertools/obtersaidamaterialnotafiscal/{adicionarFiltrosSaidaMaterialNFParaReportServer.Success}.xml"
                }
            };

            if (saidaMaterialNotaFiscalFilterModel.DataInicio != null)
            {
                parametros.Add("dataInicio", $"{saidaMaterialNotaFiscalFilterModel.DataInicio:dd/MM/yyyy}");
            }

            if (saidaMaterialNotaFiscalFilterModel.DataTermino != null)
            {
                parametros.Add("dataTermino", $"{saidaMaterialNotaFiscalFilterModel.DataTermino:dd/MM/yyyy}");
            }

            return parametros;
        }
    }
}
