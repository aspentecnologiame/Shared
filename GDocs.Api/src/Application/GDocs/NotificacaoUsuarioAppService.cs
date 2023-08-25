using ICE.GDocs.Domain.GDocs.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    internal class NotificacaoUsuarioAppService : INotificacaoUsuarioAppService
    {
        private readonly INotificacaoUsuarioService _notificacaoUsuarioService;
        public NotificacaoUsuarioAppService(INotificacaoUsuarioService notificacaoUsuarioService)
        {
            _notificacaoUsuarioService = notificacaoUsuarioService;
        }

        public async Task<TryException<int>> AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(int idNotificacaoUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioService.AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(idNotificacaoUsuario, cancellationToken);

        public async Task<TryException<NotificacaoRelatorioModel>> ObterNotificacaoNaoLidaPorIdNotificacao(int idNotificacaoUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioService.ObterNotificacaoNaoLidaPorIdNotificacao(idNotificacaoUsuario, cancellationToken);

        public async Task<TryException<IEnumerable<NotificacaoRelatorioModel>>> ObterNotificacoesNaoLidasPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioService.ObterNotificacoesNaoLidasPorIdUsuario(idUsuario, cancellationToken);

        public async Task<TryException<int>> ObterQuantidadeNotificacoesNaoLidasPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioService.ObterQuantidadeNotificacoesNaoLidasPorIdUsuario(idUsuario, cancellationToken);
    }
}
