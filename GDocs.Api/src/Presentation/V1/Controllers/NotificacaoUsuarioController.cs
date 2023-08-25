using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class NotificacaoUsuarioController : ControllerBase
    {
        private readonly INotificacaoUsuarioAppService _notificacaoUsuarioAppService;
        public NotificacaoUsuarioController(INotificacaoUsuarioAppService notificacaoUsuarioAppService)
        {
            _notificacaoUsuarioAppService = notificacaoUsuarioAppService;
        }

        [Route("ObterQuantidadeNaoLidasPorIdUsuario"), HttpGet]
        [ApiExplorerSettings(GroupName = "Notificacao")]
        [ProducesResponseType(typeof(ActionResult<int>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> ObterQuantidadeNotificacoesNaoLidasPorIdUsuario(
            Guid idUsuario,
            CancellationToken cancellationToken = default
        )
        {
            var resposta = await _notificacaoUsuarioAppService.ObterQuantidadeNotificacoesNaoLidasPorIdUsuario(idUsuario, cancellationToken);

            if (resposta.IsFailure)
                return this.Failure(resposta.Failure);

            return this.Success(resposta.Success);
        }

        [Route("ObterNaoLidasPorIdUsuario"), HttpGet]
        [ApiExplorerSettings(GroupName = "Notificacao")]
        [ProducesResponseType(typeof(ActionResult<IEnumerable<NotificacaoRelatorioModel>>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<NotificacaoRelatorioModel>>> ObterNotificacoesNaoLidasPorIdUsuario(
            Guid idUsuario,
            CancellationToken cancellationToken = default
        )
        {
            var resposta = await _notificacaoUsuarioAppService.ObterNotificacoesNaoLidasPorIdUsuario(idUsuario, cancellationToken);

            if (resposta.IsFailure)
                return this.Failure(resposta.Failure);

            return this.Success(resposta.Success);
        }

        [Route("ObterNaoLidaPorIdNotificacao"), HttpGet]
        [ApiExplorerSettings(GroupName = "Notificacao")]
        [ProducesResponseType(typeof(ActionResult<NotificacaoRelatorioModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ActionResult<NotificacaoRelatorioModel>>> ObterNotificacaoNaoLidaPorIdNotificacao(
            int idNotificacaoUsuario,
            CancellationToken cancellationToken = default
        )
        {
            var resposta = await _notificacaoUsuarioAppService.ObterNotificacaoNaoLidaPorIdNotificacao(idNotificacaoUsuario, cancellationToken);

            if (resposta.IsFailure)
                return this.Failure(resposta.Failure);

            return this.Success(resposta.Success);
        }

        [Route("AtualizarPorIdNotificacaoUsuario"), HttpPost]
        [ApiExplorerSettings(GroupName = "Notificacao")]
        [ProducesResponseType(typeof(ActionResult<int>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(
            [FromBody] int idNotificacaoUsuario,
            CancellationToken cancellationToken = default
        )
        {
            var resposta = await _notificacaoUsuarioAppService.AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(idNotificacaoUsuario, cancellationToken);

            if (resposta.IsFailure)
                return this.Failure(resposta.Failure);

            return this.Success(resposta.Success);
        }
    }
}
