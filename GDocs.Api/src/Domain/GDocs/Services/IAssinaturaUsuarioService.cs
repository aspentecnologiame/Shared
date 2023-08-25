using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    public interface IAssinaturaUsuarioService : IDomainService
    {
        Task<TryException<Return>> SalvarArquivoBase64(string arquivoBase64AssinaturaUsuario, Guid usuarioGuid, CancellationToken cancellationToken);
        Task<TryException<List<AssinaturaArmazenadaUsuarioModel>>> ObterAssinaturasDoUsuarioPorListagemDeGuid(List<Guid> listaGuid, CancellationToken cancellationToken);
    }
}