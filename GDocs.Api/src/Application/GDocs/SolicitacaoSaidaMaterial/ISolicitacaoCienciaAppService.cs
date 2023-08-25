using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoCienciaAppService : IApplicationService
    {
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<SolicitacaoCienciaModel>> ObterCienciaPeloIdentificador(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsPorUsuario(int IdSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<Return>> RegistroCienciaPorUsuario(SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken);
        Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int solicitacaoSaidaMaterialId, CancellationToken cancellationToken);

    }
}
