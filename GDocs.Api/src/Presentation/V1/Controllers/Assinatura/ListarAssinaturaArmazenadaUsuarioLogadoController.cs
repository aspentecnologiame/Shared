using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
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
    public class ListarAssinaturaArmazenadaUsuarioLogadoController : ControllerBase
    {
        private readonly IAssinaturaArmazenadaUsuarioRepository _assinaturaArmazenadaUsuarioRepository;

        public ListarAssinaturaArmazenadaUsuarioLogadoController(IAssinaturaArmazenadaUsuarioRepository assinaturaArmazenadaUsuarioRepository)
        {
            _assinaturaArmazenadaUsuarioRepository = assinaturaArmazenadaUsuarioRepository;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [HttpGet]
        [ProducesResponseType(typeof(AssinaturaArmazenadaUsuarioModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AssinaturaArmazenadaUsuarioModel>> Get(
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaArmazenadaUsuarioRepository.ListarPorUsuario(this.ObterUsuario().ActiveDirectoryId, cancellationToken);
            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        }
    }
}