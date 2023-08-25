using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ICE.GDocs.Api.V1.Controllers.Assinatura
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Assinatura/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class AtualizarDestaqueController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public AtualizarDestaqueController(IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult> Post(
            AssinaturaInformacoesModel assinaturaInformacoesModel,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaAppService.AtualizarDestaquePorPadId(assinaturaInformacoesModel.Id, assinaturaInformacoesModel.Destaque, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success();
        }
    }
}
