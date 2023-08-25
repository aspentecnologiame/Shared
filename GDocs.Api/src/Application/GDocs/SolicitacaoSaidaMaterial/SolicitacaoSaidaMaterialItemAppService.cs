using ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialItemAppService : ISolicitacaoSaidaMaterialItemAppService
    {
        private readonly ISolicitacaoSaidaMaterialItemService _solicitacaoSaidaMaterialItemService;
        public SolicitacaoSaidaMaterialItemAppService(ISolicitacaoSaidaMaterialItemService solicitacaoSaidaMaterialItemService)
        {
            _solicitacaoSaidaMaterialItemService = solicitacaoSaidaMaterialItemService;
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial,  int solicitacaoSaidaMaterialAcao, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialItemService.ObterPorIdSolicitacaoSaidaMaterial(idSolicitacaoSaidaMaterial, solicitacaoSaidaMaterialAcao, cancellationToken);

        public async Task<TryException<SolicitacaoSaidaMaterialItemModel>> Atualizar(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel)
        => await _solicitacaoSaidaMaterialItemService.Atualizar(solicitacaoSaidaMaterialItemModel);

        public async Task<TryException<Return>> Inserir(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialItemService.Inserir(solicitacaoSaidaMaterialItemModel, cancellationToken);

        public async Task<TryException<Return>> Excluir(int id)
        => await _solicitacaoSaidaMaterialItemService.Excluir(id);
    }
}
