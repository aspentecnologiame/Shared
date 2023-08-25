using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Domain.Services;
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
    public class GravarController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GravarController(
            IAssinaturaAppService assinaturaAppService,
            ISequencialService sequencialService,
            IWebHostEnvironment hostingEnvironment)
        {
            _assinaturaAppService = assinaturaAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<AssinaturaModel>> Post(
            AssinaturaModel assinaturaModel,
            CancellationToken cancellationToken = default
        )
        {
            assinaturaModel.Informacoes.UsuarioGuidAd = this.ObterUsuario().ActiveDirectoryId;

            var gravar = await _assinaturaAppService.SalvarProcesso(assinaturaModel, this.ObterUploadBasePath(_hostingEnvironment), cancellationToken);

            if (gravar.IsFailure)
                return this.Failure(gravar.Failure);

            return this.Success(gravar.Success);
        }
    }
}