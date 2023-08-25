using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialItemService : DomainService, ISolicitacaoSaidaMaterialItemService
    {
        private readonly ISolicitacaoSaidaMaterialItemRepository _solicitacaoSaidaMaterialItemRepository;

        public SolicitacaoSaidaMaterialItemService(
            IUnitOfWork unitOfWork, 
            ISolicitacaoSaidaMaterialItemRepository solicitacaoSaidaMaterialItemRepository) : base(unitOfWork)
        {
            _solicitacaoSaidaMaterialItemRepository = solicitacaoSaidaMaterialItemRepository;
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, int solicitacaoSaidaMaterialAcao, CancellationToken cancellationToken)
        {
            switch(solicitacaoSaidaMaterialAcao)
            {
                case (int)SaidaMaterialTipoAcao.RegistroSaida:
                    return await _solicitacaoSaidaMaterialItemRepository.ObterPorIdSolicitacaoSaidaMaterial(idSolicitacaoSaidaMaterial, cancellationToken);

                case (int)SaidaMaterialTipoAcao.RegistroRetorno:
                case (int)SaidaMaterialTipoAcao.SolicitacaoProrrogacao:
                case (int)SaidaMaterialTipoAcao.BaixaMaterialSemRetorno:
                    return await _solicitacaoSaidaMaterialItemRepository.ObterItemMateriaParaRetornoPorId(idSolicitacaoSaidaMaterial, cancellationToken);

                default:
                    return new TryException<IEnumerable<SolicitacaoSaidaMaterialItemModel>>();
            }
        }

        public async Task<TryException<SolicitacaoSaidaMaterialItemModel>> Atualizar(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel)
        => await _solicitacaoSaidaMaterialItemRepository.Atualizar(solicitacaoSaidaMaterialItemModel);

        public async Task<TryException<Return>> Inserir(SolicitacaoSaidaMaterialItemModel solicitacaoSaidaMaterialItemModel, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialItemRepository.Inserir(solicitacaoSaidaMaterialItemModel, cancellationToken);

        public async Task<TryException<Return>> Excluir(int id)
        => await _solicitacaoSaidaMaterialItemRepository.Excluir(id);

    }
}
