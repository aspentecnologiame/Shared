using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Service
{
    internal class SaidaItemMaterialNotaFiscalService : DomainService, ISaidaMaterialNotaFiscalItemService
    {
        private readonly ISaidaMaterialNotaFiscalItemRepository _saidaMaterialNotaFiscalItemRepository;
        public SaidaItemMaterialNotaFiscalService(IUnitOfWork unitOfWork, ISaidaMaterialNotaFiscalItemRepository saidaMaterialNotaFiscalItemRepository) : base(unitOfWork)
        {
            _saidaMaterialNotaFiscalItemRepository = saidaMaterialNotaFiscalItemRepository;
        }
        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialNotaFiscalTipoAcao acao, CancellationToken cancellationToken)
        {
            switch (acao)
            {
                case SaidaMaterialNotaFiscalTipoAcao.RegistroSaida:
                    return await _saidaMaterialNotaFiscalItemRepository.ObterPorIdSolicitacaoSaidaMaterial(idSolicitacaoSaidaMaterial, cancellationToken);

                case SaidaMaterialNotaFiscalTipoAcao.RegistroRetorno:
                case SaidaMaterialNotaFiscalTipoAcao.SolicitacaoProrrogacao:
                case SaidaMaterialNotaFiscalTipoAcao.BaixaMaterialSemRetorno:
                    return await _saidaMaterialNotaFiscalItemRepository.ObterItemMateriaParaAcaoPorId(idSolicitacaoSaidaMaterial, cancellationToken);
                default:
                    throw new BusinessException($"Não é possivel efetivar a ação pois o mesmo não esta implementado para o status: {acao.GetDescription()}");
            }

        }
    }
}
