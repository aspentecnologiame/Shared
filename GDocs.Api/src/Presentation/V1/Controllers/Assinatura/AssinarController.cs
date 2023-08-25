using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
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
    public class AssinarController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AssinarController(
            IAssinaturaAppService assinaturaAppService,
            IWebHostEnvironment hostingEnvironment)
        {
            _assinaturaAppService = assinaturaAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<AssinaturaModel>> Post(AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemAssinarModel, CancellationToken cancellationToken = default)
        {
            var assinar = await _assinaturaAppService.Assinar(this.ObterUsuario().ActiveDirectoryId, assinaturaPassoItemAssinarModel, this.ObterUploadBasePath(_hostingEnvironment), cancellationToken);
            if (assinar.IsFailure)
                return this.Failure(assinar.Failure);

            return this.Success(assinar.Success);
        }
    }
}