using AutoMapper;
using ICE.GDocs.Api.V1.Models;
using ICE.GDocs.Application;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConfiguracaoController : ControllerBase
    {
        private readonly IConfiguracaoAppService _configuracaoAppService;
        private readonly IMapper _mapper;

        public ConfiguracaoController(
            IConfiguracaoAppService configuracaoAppService,
            IMapper mapper
        )
        {
            _configuracaoAppService = configuracaoAppService;
            _mapper = mapper;
        }

        [HttpGet("{chave}")]
        [ApiExplorerSettings(GroupName = "Configuracao")]
        public async Task<ActionResult<Configuracao>> Get(
            string chave,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _configuracaoAppService.ObterConfiguracaoRepositorio(chave, cancellationToken);

            if (result.IsFailure)
                return this.Failure(result.Failure);

            var configuracao = result.Success;

            if (configuracao == null)
                return NotFound();

            return this.Success(_mapper.Map<Configuracao>(configuracao));
        }
    }
}
