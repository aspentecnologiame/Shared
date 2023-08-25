using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Data;

namespace ICE.GDocs.Api.V1.Controllers.Cache
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Cache/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class GravarPassoCacheController : ControllerBase
    {
        private readonly IGravarPassoCacheAppService _gravarPassoCacheAppService;

        public GravarPassoCacheController(IGravarPassoCacheAppService gravarPassoCacheAppService)
        {
            _gravarPassoCacheAppService = gravarPassoCacheAppService;
        }

        [ApiExplorerSettings(GroupName = "Cache")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<int>> Post([FromBody] int padId, CancellationToken cancellationToken = default)
        {
            var gravar = await _gravarPassoCacheAppService.GravarPassoUsuario(padId, this.ObterUsuario().ActiveDirectoryId, cancellationToken);

            if (gravar.IsFailure)
                return this.Failure(gravar.Failure);

            return this.Success(gravar.Success);
        }
    }

}
