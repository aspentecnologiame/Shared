using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI1548/consulta")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultaFiltrarController : ControllerBase
    {
        private readonly IAuthorizationService _authService;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ConsultaFiltrarController(
            IAuthorizationService authService,
            IDocumentoFI1548AppService documentoFI1548AppService
            )
        {
            _authService = authService;
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<DocumentoFI1548Model>>> Post(
            DocumentoFI1548FilterModel filtro,
            CancellationToken cancellationToken = default
        )
        {
            filtro.AutoresADId = await this.HasRoleAsync(_authService, "fi1548:consultar:todosusuarios")
                ? filtro.AutoresADId : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };

            filtro.UsuarioLogadoAd = this.ObterUsuario().ActiveDirectoryId;

            var response = await _documentoFI1548AppService.ObterPorNumeroTipoPagamentoAutorPeriodo(filtro, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var documentoList = response.Success;

            if (documentoList == null)
                return NotFound();

            return this.Success(documentoList);
        }
    }
}
