using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services
{
    internal class GravarPassoCacheService :  IGravarPassoCacheService
    {

        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;
        private readonly IGDocsCacheExternalService _gDocsCacheExternalService;

        public GravarPassoCacheService(IUsuarioService usuarioService, IGDocsCacheExternalService gDocsCacheExternalService, IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _gDocsCacheExternalService = gDocsCacheExternalService;
            _configuration = configuration;
        }

        public async Task<TryException<int>> GravarPassoUsuario(int padId, Guid usuarioId, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioService.ObterUsuarioActiveDirectoryPorId(usuarioId, cancellationToken);

            var passoDownLoad = new PassoUsuarioDownload
            {
                PadId = padId,
                UsuarioId = usuarioId,
                NomeUsuario = usuario.Success.Nome,
            };
            return await _gDocsCacheExternalService.AdicionarPassoUsuarioDownload(passoDownLoad, TimeSpan.FromDays(_configuration.GetValue("Cache:qtdDiaTempoExpiracao", 1)));
        }

        public async Task<TryException<PassoUsuarioDownload>> ObterPassoPorId(int chave)
        {
            return await _gDocsCacheExternalService.ObterPassoUsuarioDownload(chave);
        }

        public async Task RemoverPassoPorId(int chave, Guid usuarioId)
        {
            await _gDocsCacheExternalService.RemoverPassoUsuarioDownload(chave,usuarioId);
        }
    }
}
