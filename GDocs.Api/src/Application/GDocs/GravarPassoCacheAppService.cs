using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    internal class GravarPassoCacheAppService : IGravarPassoCacheAppService
    {
        private readonly IGravarPassoCacheAgregationAppService _gravarPassoCacheAgregationAppService;

        public GravarPassoCacheAppService(IGravarPassoCacheAgregationAppService gravarPassoCacheAgregationAppService)
        {
            _gravarPassoCacheAgregationAppService = gravarPassoCacheAgregationAppService;
        }

        public async Task<TryException<int>> GravarPassoUsuario(int padId, Guid usuarioId, CancellationToken cancellationToken) => 
            await _gravarPassoCacheAgregationAppService.GravarPassoCacheService.GravarPassoUsuario(padId, usuarioId, cancellationToken);


        public async Task<TryException<PassoUsuarioDownload>> ObterPassoPorId(int chave) => await _gravarPassoCacheAgregationAppService.GravarPassoCacheService.ObterPassoPorId(chave);

        public async Task RemoverPassoPorId(int chave ,Guid usuarioId) => await _gravarPassoCacheAgregationAppService.GravarPassoCacheService.RemoverPassoPorId(chave,usuarioId);
    }
}
