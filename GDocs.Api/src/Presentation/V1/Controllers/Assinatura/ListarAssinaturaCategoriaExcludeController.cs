using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
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
    public class ListarAssinaturaCategoriaExcludeController : ControllerBase
    {
        private readonly IAssinaturaCategoriaRepository _assinaturaCategoriaRepository;
        private readonly IConfiguration _configuration;

        public ListarAssinaturaCategoriaExcludeController(
            IAssinaturaCategoriaRepository assinaturaCategoriaRepository,
            IConfiguration configuration)
        {
            _assinaturaCategoriaRepository = assinaturaCategoriaRepository;
            _configuration = configuration;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [HttpGet("{exclude}")]
        [ProducesResponseType(typeof(IEnumerable<AssinaturaCategoriaModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<AssinaturaCategoriaModel>>> Get(
            bool exclude = true,
            CancellationToken cancellationToken = default
        )
        {
            var listaAssinaturaAdicionarCategoriaExclude = exclude ? (_configuration.GetSection("AssinaturaAdicionarCategoriaExclude")?.Get<int[]>()
                      ?? Array.Empty<int>()) : Array.Empty<int>();

            var response = await _assinaturaCategoriaRepository.Listar(listaAssinaturaAdicionarCategoriaExclude, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        }
    }
}
