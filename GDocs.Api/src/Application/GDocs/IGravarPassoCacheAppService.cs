using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    public interface IGravarPassoCacheAppService: IApplicationService
    {
        Task<TryException<int>> GravarPassoUsuario(int padId, Guid usuarioId, CancellationToken cancellationToken);

        Task<TryException<PassoUsuarioDownload>> ObterPassoPorId(int chave);

        Task RemoverPassoPorId(int chave , Guid usuarioId);
    }
}
