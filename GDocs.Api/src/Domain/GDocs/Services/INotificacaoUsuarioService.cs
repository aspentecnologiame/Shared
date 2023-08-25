using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services
{
    public interface INotificacaoUsuarioService : IDomainService
    {
        Task<TryException<IEnumerable<NotificacaoRelatorioModel>>> ObterNotificacoesNaoLidasPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken);
        Task<TryException<NotificacaoRelatorioModel>> ObterNotificacaoNaoLidaPorIdNotificacao(int idNotificacaoUsuario, CancellationToken cancellationToken);
        Task<TryException<int>> ObterQuantidadeNotificacoesNaoLidasPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken);
        Task<TryException<int>> AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(int idNotificacaoUsuario, CancellationToken cancellationToken);
    }
}
