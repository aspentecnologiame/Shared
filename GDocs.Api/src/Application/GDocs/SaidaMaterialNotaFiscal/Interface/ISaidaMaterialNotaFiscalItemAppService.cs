using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface
{
    public interface ISaidaMaterialNotaFiscalItemAppService : IApplicationService
    {
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalItemModel>>> ObterPorIdSolicitacaoSaidaMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialNotaFiscalTipoAcao acao, CancellationToken cancellationToken);
    }
}
