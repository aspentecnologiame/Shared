using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoCienciaService : IDomainService
    {
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<bool>> RegistroCienciaPorUsuario(SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsPorUsuario(int IdSolicitacaoCiencia, CancellationToken cancellationToken);

        Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int solicitacaoSaidaMaterialId, CancellationToken cancellationToken);
    }
}
