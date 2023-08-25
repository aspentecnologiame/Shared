using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SolicitacaoSaidaMaterial.Ciencia
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/Ciencia/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarDocumentosPendentesCienciaPeloUsuarioController : ControllerBase
    {
        private readonly ISolicitacaoCienciaAppService _solicitacaoCienciaAppService;

        public ListarDocumentosPendentesCienciaPeloUsuarioController(
            ISolicitacaoCienciaAppService solicitacaoCienciaAppService
        )
        {
            _solicitacaoCienciaAppService = solicitacaoCienciaAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:ciencia:pendente", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<ProcessoAssinaturaDocumentoModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProcessoAssinaturaDocumentoModel>>> Get(
            CancellationToken cancellationToken = default
        )
        {
            var usuario = this.ObterUsuario();
            
            var listaCienciasPendentes = await _solicitacaoCienciaAppService.ListarCienciasPendentesDeAprovacaoPeloUsuario(usuario.ActiveDirectoryId, cancellationToken);

            if (listaCienciasPendentes.IsFailure)
                return this.Failure(listaCienciasPendentes.Failure);

            return this.Success(listaCienciasPendentes.Success);
        }
    }
}
