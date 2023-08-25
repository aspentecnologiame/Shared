using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System;
using ICE.GDocs.Infra.CrossCutting.Models;
using System.Net;

namespace ICE.GDocs.Api.V1.Controllers.Cache
{

    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Cache/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class LimparPassoCacheController : ControllerBase
    {
        private readonly IGravarPassoCacheAppService _gravarPassoCacheAppService;


        public LimparPassoCacheController(IGravarPassoCacheAppService gravarPassoCacheAppService)
        {
            _gravarPassoCacheAppService = gravarPassoCacheAppService;
        }

        [ApiExplorerSettings(GroupName = "Cache")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task Delete([FromBody] int padId, CancellationToken cancellationToken = default) => await _gravarPassoCacheAppService.RemoverPassoPorId(padId, this.ObterUsuario().ActiveDirectoryId);

    }
}
