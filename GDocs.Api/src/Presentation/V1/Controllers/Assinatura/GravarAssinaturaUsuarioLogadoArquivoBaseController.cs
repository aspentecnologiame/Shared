using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class GravarAssinaturaUsuarioLogadoArquivoBaseController : ControllerBase
    {
        private readonly IAssinaturaUsuarioService _assinaturaUsuarioAppService;

        public GravarAssinaturaUsuarioLogadoArquivoBaseController(IAssinaturaUsuarioService assinaturaUsuarioAppService, IWebHostEnvironment hostingEnvironment)
        {
            _assinaturaUsuarioAppService = assinaturaUsuarioAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [HttpPost("")]
        public async Task<ActionResult<Return>> Post(
                      AssinaturaBase64UsuarioModel arquivoBase64AssinaturaUsuario,
            CancellationToken cancellationToken = default
        )
        {
            var gravar = await _assinaturaUsuarioAppService.SalvarArquivoBase64(arquivoBase64AssinaturaUsuario.ArquivoBase64AssinaturaUsuario, this.ObterUsuario().ActiveDirectoryId, cancellationToken);

            if (gravar.IsFailure)
                return this.Failure(gravar.Failure);

            return Return.Empty;
        }
    }
}