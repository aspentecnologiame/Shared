using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Service
{
    public class SaidaMaterialNotaFiscalItemAppService : ISaidaMaterialNotaFiscalItemAppService
    {

        private readonly ISaidaMaterialNotaFiscalItemService _saidaMaterialNotaFiscalItemService;
        public SaidaMaterialNotaFiscalItemAppService(ISaidaMaterialNotaFiscalItemService saidaMaterialNotaFiscalItemService)
        {
            _saidaMaterialNotaFiscalItemService = saidaMaterialNotaFiscalItemService;
        }
        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialNotaFiscalTipoAcao acao, CancellationToken cancellationToken) =>
            await _saidaMaterialNotaFiscalItemService.ObterPorIdSolicitacaoSaidaMaterial(idSolicitacaoSaidaMaterial, acao, cancellationToken);
        
    }
}
