using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models;

namespace ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal
{
    public interface ISaidaMaterialNotaFiscalItemRepository : IRepository
    {
        Task<TryException<Return>> Inserir(SaidaMaterialNotaFiscalItemModel itemSaidaMaterialNFModel, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterItemMateriaParaAcaoPorId(int idSolicitacaoSaidaMaterialNf, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemProrrogadoModel>>> ObterItensProrrogadosEAprovados(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<SaidaMaterialNotaFiscalItemModel>> ObterItemPorId(int idSaidaMaterialNotaFiscalItem, CancellationToken cancellationToken);
        Task<TryException<SaidaMaterialNotaFiscalItemModel>> Atualizar(SaidaMaterialNotaFiscalItemModel saidaMaterialNotaFiscalItemModel);
        Task<TryException<Return>> Excluir(int id);
        Task<TryException<int>> ObterQuantidadeItemComBaixa(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterItemMateriaParaRetornoPorId(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<Return>> DesativarItemMaterial(IEnumerable<int> smnfiIdt, CancellationToken cancellationToken);

    }
}
