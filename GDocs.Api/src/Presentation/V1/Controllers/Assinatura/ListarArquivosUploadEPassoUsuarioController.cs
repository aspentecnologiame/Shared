using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
    public class ListarArquivosUploadEPassoUsuarioController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public ListarArquivosUploadEPassoUsuarioController(
            IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [HttpGet("{processoAssinaturaDocumentoId}")]
        [ProducesResponseType(typeof(AssinaturaArquivoAssinaturaPassoAssinanteModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AssinaturaArquivoAssinaturaPassoAssinanteModel>> Get(
            int processoAssinaturaDocumentoId,
            CancellationToken cancellationToken = default
        )
        {
            var responseArquivo = await _assinaturaAppService.ListarArquivosUploadPorPadId(processoAssinaturaDocumentoId, cancellationToken);
            if (responseArquivo.IsFailure)
                return this.Failure(responseArquivo.Failure);

            if (responseArquivo.Success == null)
                return NotFound();

            var responsePassoUsuario = await _assinaturaAppService.ObterProcessoComPassosParaAssinatura(processoAssinaturaDocumentoId, cancellationToken);
            if (responsePassoUsuario.IsFailure)
                return this.Failure(responsePassoUsuario.Failure);

            return this.Success(new AssinaturaArquivoAssinaturaPassoAssinanteModel(responseArquivo.Success, responsePassoUsuario.Success.Passos.Itens.SelectMany(passo => passo.Usuarios)));
        }
    }
}