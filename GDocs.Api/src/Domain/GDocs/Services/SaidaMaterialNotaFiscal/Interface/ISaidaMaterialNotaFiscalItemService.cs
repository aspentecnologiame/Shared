using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface
{
    public interface ISaidaMaterialNotaFiscalItemService : IDomainService
    {
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialNotaFiscalTipoAcao acao, CancellationToken cancellationToken);
    }
}
