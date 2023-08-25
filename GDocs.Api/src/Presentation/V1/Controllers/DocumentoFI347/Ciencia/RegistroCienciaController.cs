using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SolicitacaoSaidaMaterial.Ciencia
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/Ciencia/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class RegistroCienciaController : ControllerBase
    {
        private readonly ISolicitacaoCienciaAppService _solicitacaoCienciaAppService;


        public RegistroCienciaController(
            ISolicitacaoCienciaAppService solicitacaoCienciaAppService
        )
        {
            _solicitacaoCienciaAppService = solicitacaoCienciaAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AllowAnonymous]
        [HttpPost("")]
        [ProducesResponseType(typeof(SolicitacaoCienciaModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<SolicitacaoCienciaModel>> Post(
            SolicitacaoCienciaModel solicitacaoCienciaModel,
            CancellationToken cancellationToken = default
        )
        {
            var regristroCiencia = await _solicitacaoCienciaAppService.RegistroCienciaPorUsuario(solicitacaoCienciaModel, this.ObterUsuario(), cancellationToken);

            if (regristroCiencia.IsFailure)
                return this.Failure(regristroCiencia.Failure);

            return this.Success(regristroCiencia.Success);
        }
    }
}
