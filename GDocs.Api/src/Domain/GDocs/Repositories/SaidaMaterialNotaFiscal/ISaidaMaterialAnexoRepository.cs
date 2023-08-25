using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;

namespace ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal
{
    public interface ISaidaMaterialAnexoRepository :IRepository
    {
        Task<TryException<SaidaMaterialArquivoModel>> Salvar(SaidaMaterialArquivoModel saidaMaterialArquivoModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialArquivoModel>>> ListarAnexo(int saidaMaterialId,CancellationToken cancellationToken);
        Task<TryException<SaidaMaterialArquivoModel>> ObterAnexoPorIdEhTipo(int saidaMaterialId, TipoDocAnexo tipoDocAnexo, CancellationToken cancellationToken);
        Task<TryException<Return>> InativaAnexo(SaidaMaterialArquivoModel saidaMaterialArquivoModel, CancellationToken cancellationToken);
        Task<TryException<int>> CheckarDuplicidade(int smnfIdt, int smnfNumeroDocumento, CancellationToken cancellationToken);
    }
}
