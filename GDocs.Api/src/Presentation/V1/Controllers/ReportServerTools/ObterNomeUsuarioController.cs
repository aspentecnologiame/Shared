using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.ReportServerTools
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/ReportServerTools/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterNomeUsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public ObterNomeUsuarioController(
            IUsuarioService usuarioService
        )
        {
            _usuarioService = usuarioService;
        }

        [ApiExplorerSettings(GroupName = "Report Server Tools")]
        [HttpGet("{usuarioGuid}")]
        [ResponseCache(Duration = 7200, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "usuarioGuid" })]
        public async Task<ActionResult<string>> Get(
            Guid usuarioGuid,
            CancellationToken cancellationToken = default
        )
        {
            var obterUsuarioActiveDirectoryPorId = await _usuarioService.ObterUsuarioActiveDirectoryPorId(usuarioGuid, cancellationToken);

            if (obterUsuarioActiveDirectoryPorId.IsFailure)
                return this.Failure(obterUsuarioActiveDirectoryPorId.Failure);

            return this.Success(obterUsuarioActiveDirectoryPorId.Success?.Nome ?? usuarioGuid.ToString());
        }
    }
}
