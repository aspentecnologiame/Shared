using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta
{
    [ApiController]
    [AuthorizeBearer(Roles = "fi1548:consultar")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentoFI1548/consulta/cancelar")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultaCancelarController : ControllerBase
    {
        private readonly IAuthorizationService _authService;        
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ConsultaCancelarController(
            IAuthorizationService authService,            
            IDocumentoFI1548AppService documentoFI1548AppService
            )
        {
            _authService = authService;            
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [HttpPost("")]
        public async Task<ActionResult<DocumentoFI1548Model>> Post(int documentoId, string motivo, CancellationToken cancellationToken = default)
        {
            bool permissaoCancelarTodos = await this.HasRoleAsync(_authService, "fi1548:consultar:todosusuarios:cancelartodos");

            var response = await _documentoFI1548AppService.CancelarOuSolicitarCiencia(documentoId, motivo, this.ObterUsuario(), permissaoCancelarTodos, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);
        }
    }
}
