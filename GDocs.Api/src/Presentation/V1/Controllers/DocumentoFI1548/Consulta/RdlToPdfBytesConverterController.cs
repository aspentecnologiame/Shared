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

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentofi1548/consulta/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class RdlToPdfBytesConverterController : ControllerBase
    {
        private readonly IRdlToPdfBytesConverterExternalService _rdlToPdfBytesConverterExternalService;
        private readonly IConfiguration _configuration;
        private readonly IAuthorizationService _authService;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;

        public RdlToPdfBytesConverterController(
            IRdlToPdfBytesConverterExternalService rdlToPdfBytesConverterExternalService,
            IConfiguration configuration,
            IAuthorizationService authService,
            IGDocsCacheExternalService gDocsCacheExternalService
        )
        {
            _rdlToPdfBytesConverterExternalService = rdlToPdfBytesConverterExternalService;
            _configuration = configuration;
            _authService = authService;
            _gDocsCacheExternalService = gDocsCacheExternalService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<byte[]>> PostAsync(
            DocumentoFI1548FilterModel documentoFI1548FilterModel
        )
        {
            documentoFI1548FilterModel.AutoresADId = await this.HasRoleAsync(_authService, "fi1548:consultar:todosusuarios")
                    ? documentoFI1548FilterModel.AutoresADId : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };
            documentoFI1548FilterModel.UsuarioLogadoAd = this.ObterUsuario().ActiveDirectoryId;

            var construirParametrosParaReport = await ConstruirParametrosParaReport(documentoFI1548FilterModel);
            if (construirParametrosParaReport.IsFailure)
                return this.Failure(construirParametrosParaReport.Failure);

            var response = _rdlToPdfBytesConverterExternalService.Converter(
                _configuration.GetValue("DocumentoFI1548:RdlPath", string.Empty),
                construirParametrosParaReport.Success
            );

            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        }

        private async Task<TryException<Dictionary<string, string>>> ConstruirParametrosParaReport(DocumentoFI1548FilterModel documentoFI1548FilterModel)
        {
            var adicionarFiltrosDocumentoFI1548ParaReportServer = await _gDocsCacheExternalService.AdicionarFiltrosDocumentoFI1548ParaReportServer(documentoFI1548FilterModel, TimeSpan.FromMinutes(1));
            if (adicionarFiltrosDocumentoFI1548ParaReportServer.IsFailure)
                return adicionarFiltrosDocumentoFI1548ParaReportServer.Failure;

            var parametros = new Dictionary<string, string> {
                {
                    "xmlConnectString",
                    $"{Request.Scheme}://{Request.Host}/api/v1/reportservertools/obterdadosstatuspagamento/{adicionarFiltrosDocumentoFI1548ParaReportServer.Success}.xml"
                }
            };

            if (documentoFI1548FilterModel.DataInicio != null)
            {
                parametros.Add("dataInicio", $"{documentoFI1548FilterModel.DataInicio:dd/MM/yyyy}");
            }

            if (documentoFI1548FilterModel.DataTermino != null)
            {
                parametros.Add("dataTermino", $"{documentoFI1548FilterModel.DataTermino:dd/MM/yyyy}");
            }

            return parametros;
        }
    }
}