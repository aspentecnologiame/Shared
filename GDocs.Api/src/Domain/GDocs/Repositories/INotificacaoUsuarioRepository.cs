using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories
{
    public interface INotificacaoUsuarioRepository : IRepository
    {
        Task<TryException<IEnumerable<NotificacaoRelatorioModel>>> ObterRelatoriosNaoLidosPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken);
        Task<TryException<NotificacaoRelatorioModel>> ObterRelatorioNaoLidoPorIdNotificacao(int idNotificacaoUsuario, CancellationToken cancellationToken);
        Task<TryException<int>> ObterQuantidadeRelatoriosNaoLidosPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken);
        Task<TryException<int>> AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(int idNotificacaoUsuario, CancellationToken cancellationToken);
        Task<TryException<RelatorioModel>> ObterParametrosDoRelatorio(CancellationToken cancellationToken, RelatorioNotificacao relatorioNotificacao);
    }
}
