using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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
    public class ObterCienciaPeloIdentificadorController : ControllerBase
    {
        private readonly ISolicitacaoCienciaAppService _solicitacaoCienciaAppService;
        private readonly ISolicitacaoSaidaMaterialAcaoAppService _solicitacaoSaidaMaterialAcaoAppService;

        public ObterCienciaPeloIdentificadorController(
            ISolicitacaoCienciaAppService solicitacaoCienciaAppService,
            ISolicitacaoSaidaMaterialAcaoAppService solicitacaoSaidaMaterialAcaoAppService
        )
        {
            _solicitacaoCienciaAppService = solicitacaoCienciaAppService;
            _solicitacaoSaidaMaterialAcaoAppService = solicitacaoSaidaMaterialAcaoAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AllowAnonymous]
        [HttpGet("")]
        [ProducesResponseType(typeof(ObterCienciaPeloIdentificadorResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ObterCienciaPeloIdentificadorResponseModel>> Get(
            [FromQuery] int idSolicitacaoCiencia,
            CancellationToken cancellationToken = default
        )
        {
            var obterCiencia = await _solicitacaoCienciaAppService.ObterCienciaPeloIdentificador(idSolicitacaoCiencia, cancellationToken);

            if (obterCiencia.IsFailure)
                return this.Failure(obterCiencia.Failure);

            var ciencia = obterCiencia.Success;

            var historicoProrrogacao = await _solicitacaoSaidaMaterialAcaoAppService.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(
                ciencia.IdSolicitacaoSaidaMaterial,
                 SaidaMaterialTipoAcao.SolicitacaoProrrogacao,
                cancellationToken);

            if (historicoProrrogacao.IsFailure)
                return this.Failure(historicoProrrogacao.Failure);


            var aprovadorObservacao = await _solicitacaoCienciaAppService.ListarObsPorUsuario(idSolicitacaoCiencia, cancellationToken);
            if (aprovadorObservacao.IsFailure)
                return this.Failure(obterCiencia.Failure);

            return this.Success(
                new ObterCienciaPeloIdentificadorResponseModel() { 
                    SolicitacaoCiencia = ciencia,
                    HistoricoProrrogacoes = historicoProrrogacao.Success,
                    CienciaUsuarioAprovacao = aprovadorObservacao.Success
                });
        }
    }
}
