using ICE.GDocs.Application;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class PerfilController : ControllerBase
    {
        private readonly IAcessoAppService _acessoAppService;

        public PerfilController(
            IAcessoAppService acessoAppService
        )
        {
            _acessoAppService = acessoAppService;
        }

        [HttpGet]
        [ApiExplorerSettings(GroupName = "Perfil")]
        [ProducesResponseType(typeof(IEnumerable<PerfilModel>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<PerfilModel>> Get(CancellationToken cancellationToken = default)
        {
            var perfis = await _acessoAppService.ListarTodosOsPerfisAtivos(cancellationToken);

            return perfis.Success;
        }
    }
}