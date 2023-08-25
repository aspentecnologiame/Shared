using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public class CancelarProcessoController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public CancelarProcessoController(IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<AssinaturaInformacoesModel>> Post(
            AssinaturaInformacoesModel model,
            CancellationToken cancellationToken = default
        )
        {
            DocumentoFI1548FilterModel filtro = new DocumentoFI1548FilterModel();
            filtro.Id = model.NumeroDocumento??0;

            

            var cancelarProcesso = await _assinaturaAppService.CancelarProcesso(model, cancellationToken, filtro, this.ObterUsuario(), true);

            if (cancelarProcesso.IsFailure)
                return this.Failure(cancelarProcesso.Failure);

            return this.Success(cancelarProcesso.Success);
        }
    }
}