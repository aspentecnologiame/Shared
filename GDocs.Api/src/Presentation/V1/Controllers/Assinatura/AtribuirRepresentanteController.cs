using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public class AtribuirRepresentanteController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public AtribuirRepresentanteController(IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> Post(
            AtribuirRepresentanteModel model,
            CancellationToken cancellationToken = default
        )
        {
            var atribuirRepresentantes = await _assinaturaAppService.AtribuirRepresentantes(model.Assinantes, cancellationToken);

            if (atribuirRepresentantes.IsFailure)
                return this.Failure(atribuirRepresentantes.Failure);

            return this.Success(atribuirRepresentantes.Success);
        }
    }
}