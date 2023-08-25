using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialItemRepository : IRepository
    {
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterialRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemProrrogadoModel>>> ObterItensProrrogadosEAprovados(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialItemModel>> ObterItemPorId(int idSolicitacaoSaidaMaterialItem, CancellationToken cancellationToken);
        Task<TryException<Return>> Inserir(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialItemModel>> Atualizar(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel);
        Task<TryException<Return>> Excluir(int id);
        Task<TryException<int>> ObterQuantidadeItemComBaixa(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterItemMateriaParaRetornoPorId(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<Return>> DesativarItemMaterial(IEnumerable<int> ssmiIdt, CancellationToken cancellationToken);
    }
}
