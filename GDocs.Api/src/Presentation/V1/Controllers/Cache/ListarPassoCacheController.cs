using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.Cache
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Cache/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarPassoCacheController : ControllerBase
    {
        private readonly IGravarPassoCacheAppService _gravarPassoCacheAppService;

        public ListarPassoCacheController(IGravarPassoCacheAppService gravarPassoCacheAppService)
        {
            _gravarPassoCacheAppService = gravarPassoCacheAppService;
        }

        [ApiExplorerSettings(GroupName = "Cache")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<PassoUsuarioDownload>> Get(int padId)
        {
            var obter = await _gravarPassoCacheAppService.ObterPassoPorId(padId);


            if (obter.IsFailure || obter.Success == default)
                return NotFound();

            return this.Success(obter.Success);
        }
    }
}
