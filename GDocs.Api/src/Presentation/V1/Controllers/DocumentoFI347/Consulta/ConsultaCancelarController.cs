using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SolicitacaoSaidaMaterial.Consulta
{
    [ApiController]
    [AuthorizeBearer(Roles = "fi347:consultar")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentoFI347/consulta/cancelar")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultaCancelarController : ControllerBase
    {
        private readonly IAuthorizationService _authService;
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;

        public ConsultaCancelarController(
            IAuthorizationService authService,
            ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService)
        {
            _authService = authService;
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [HttpPost("")]
        public async Task<ActionResult<DocumentoFI1548Model>> Post(
            int solicitacaoSaidaMaterialId,
            string motivo,
            CancellationToken cancellationToken = default
        )
        {
            bool permissaoCancelarTodos = await this.HasRoleAsync(_authService, "fi347:consultar:todosusuarios:cancelartodos");

            var cancelar = await _solicitacaoSaidaMaterialAppService.CancelarOuSolicitarCiencia(solicitacaoSaidaMaterialId, motivo, this.ObterUsuario(), permissaoCancelarTodos, cancellationToken);

            if (cancelar.IsFailure)
                return this.Failure(cancelar.Failure);

            return this.Success(cancelar.Success);
        }
    }
}
