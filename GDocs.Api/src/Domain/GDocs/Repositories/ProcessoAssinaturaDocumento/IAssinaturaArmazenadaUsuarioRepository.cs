using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento
{
    public interface IAssinaturaArmazenadaUsuarioRepository : IRepository
    {
        Task<TryException<AssinaturaArmazenadaUsuarioModel>> ListarPorUsuario(Guid guid, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<AssinaturaArmazenadaUsuarioModel>>> ListarPorListagemDeUsuario(List<Guid> listaGuid, CancellationToken cancellationToken);
        Task<TryException<Return>> Salvar(long binarioId, Guid usuarioGuid, CancellationToken cancellationToken);
    }
}