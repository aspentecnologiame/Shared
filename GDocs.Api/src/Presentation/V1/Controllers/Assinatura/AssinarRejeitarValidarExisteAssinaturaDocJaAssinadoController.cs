using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
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
    public class AssinarRejeitarValidarExisteAssinaturaDocJaAssinadoController : ControllerBase
    {
        private readonly IAssinaturaArmazenadaUsuarioRepository _assinaturaArmazenadaUsuarioRepository;
        private readonly IAssinaturaService _assinaturaService;

        public AssinarRejeitarValidarExisteAssinaturaDocJaAssinadoController(
            IAssinaturaArmazenadaUsuarioRepository assinaturaArmazenadaUsuarioRepository,
            IAssinaturaService assinaturaService)
        {
            _assinaturaArmazenadaUsuarioRepository = assinaturaArmazenadaUsuarioRepository;
            _assinaturaService = assinaturaService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> Post(
            IEnumerable<int> listaDeprocessoAssinaturaDocumentoId,
            CancellationToken cancellationToken = default
        )
        {
            var responseAssinaturaArmazenada = await _assinaturaArmazenadaUsuarioRepository.ListarPorUsuario(this.ObterUsuario().ActiveDirectoryId, cancellationToken);
            if (responseAssinaturaArmazenada.IsFailure || responseAssinaturaArmazenada.Success == null)
                return NotFound("Não é possível realizar a aprovação, pois você não possui uma assinatura cadastrada no sistema");


            var responseValidarSeOUsuarioJaAssinouAlgumDosProcessos = await _assinaturaService.ValidarSeOUsuarioJaAssinouAlgumDosProcessos(
                this.ObterUsuario().ActiveDirectoryId,
                listaDeprocessoAssinaturaDocumentoId,
                Infra.CrossCutting.Models.Enums.StatusAssinaturaDocumentoPassoUsuario.AguardandoAssinatura,
                cancellationToken);

            if (responseValidarSeOUsuarioJaAssinouAlgumDosProcessos.IsFailure)
                return this.Failure(responseValidarSeOUsuarioJaAssinouAlgumDosProcessos.Failure);

            return this.Success(true);
        }
    }
}