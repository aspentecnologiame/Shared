using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialItemAppService : IApplicationService
    {
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, int solicitacaoSaidaMaterialAcao, CancellationToken cancellationToken);
        Task<TryException<Return>> Inserir(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoSaidaMaterialItemModel>> Atualizar(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel);
        Task<TryException<Return>> Excluir(int id);
    }
}
