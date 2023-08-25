using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    internal class UsuarioService : DomainService, IUsuarioService
    {
        private const int ID_PERFIL_DEFAULT = 1;

        private readonly IPerfilRepository _perfilRepository;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;

        public UsuarioService(
            IUnitOfWork unitOfWork,
            IPerfilRepository perfilRepository,
            IActiveDirectoryExternalService activeDirectoryExternalService
        ) : base(unitOfWork)
        {
            _perfilRepository = perfilRepository;
            _activeDirectoryExternalService = activeDirectoryExternalService;
        }

        public async Task<TryException<UsuarioModel>> AutenticarUsuario(string nomeUsuario, string senha, CancellationToken cancellationToken)
        {
            var usuarioAd = _activeDirectoryExternalService.AutenticarUsuario(nomeUsuario, senha);

            if (usuarioAd.IsFailure)
                return usuarioAd.Failure;

            var usuario = usuarioAd.Success;

            var usuarioPerfisDb = await ObterPermissoesUsuarioOuAdicionarUsuarioPorActiveDirectoryId(usuario.Guid, cancellationToken);

            if (usuarioPerfisDb.IsFailure)
                return usuarioPerfisDb.Failure;

            var usuarioPerfis = usuarioPerfisDb.Success;

            usuarioPerfis.Nome = usuario.Nome;
            usuarioPerfis.UsuarioDeRede = usuario.UsuarioDeRede;
            usuarioPerfis.Email = usuario.Email;

            return usuarioPerfis;
        }

        public async Task<TryException<UsuarioModel>> ObterPermissoesUsuarioPorActiveDirectoryId(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var usuarioPerfisDb = await _perfilRepository.ListarPerfisUsuarioPorGuid(activeDirectoryId, cancellationToken);

            if (usuarioPerfisDb.IsFailure)
                return usuarioPerfisDb.Failure;

            var usuarioPerfis = usuarioPerfisDb.Success;

            if (usuarioPerfis == null)
                return new BusinessException("usuario-sem-permissao", "Usuário sem permissão de acesso.");

            var funcionalidadesPermissoes = await _perfilRepository.ListarFuncionalidadesPorPerfis(usuarioPerfis.Perfis.Select(per => per.Id).AsList(), cancellationToken);

            if (funcionalidadesPermissoes.IsSuccess)
                usuarioPerfis.Permissoes = funcionalidadesPermissoes.Success;

            return usuarioPerfis;
        }

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuariosAtivos(string nome, int IdPerfil, CancellationToken cancellationToken)
        {
            var usuarioPerfilDb = await _perfilRepository.ListarPerfisUsuariosPorPerfil(IdPerfil, cancellationToken);
            if (usuarioPerfilDb.IsFailure)
                return usuarioPerfilDb.Failure;

            var usuariosPerfis = usuarioPerfilDb.Success;

            var usuariosAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(nome, usuariosPerfis.Select(s => s.ActiveDirectoryId).AsList());
            if (usuariosAd.IsFailure)
                return usuariosAd.Failure;

            var usuarios = usuariosAd.Success;

            usuariosPerfis = (from usu in usuariosPerfis
                              join usuAd in usuarios on usu.ActiveDirectoryId equals usuAd.Guid
                              select new
                              {
                                  usu,
                                  usuAd
                              })
                        .Select(obj =>
                        {
                            obj.usu.Nome = obj.usuAd.Nome;
                            obj.usu.UsuarioDeRede = obj.usuAd.UsuarioDeRede;
                            obj.usu.Email = obj.usuAd.Email;

                            obj.usu.Perfis = obj.usu.Perfis.Where(per => per.Id != ID_PERFIL_DEFAULT);

                            return obj.usu;
                        });

            usuariosPerfis = usuariosPerfis.OrderBy(usu => usu.Nome);

            return usuariosPerfis.ToCollection();
        }

        public async Task<TryException<UsuarioModel>> InserirUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken)
            => await SalvarPerfisUsuario(usuarioModel, cancellationToken);

        public async Task<TryException<UsuarioModel>> AlterarUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken)
            => await SalvarPerfisUsuario(usuarioModel, cancellationToken);

        private async Task<TryException<UsuarioModel>> SalvarPerfisUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            var users = await _perfilRepository.SalvarPerfisUsuario(usuarioModel, cancellationToken);
            if (users.IsFailure)
                return users.Failure;

            return users.Success;
        }

        public async Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(string nome, CancellationToken cancellationToken)
        {
            var usersAD = _activeDirectoryExternalService.GetActiveDirectoryUsers(nome, new List<Guid>());

            if (usersAD.IsFailure)
                return usersAD.Failure;

            var usuariosAd = usersAD.Success;

            return await Task.FromResult(usuariosAd.AsList());
        }

        public async Task<TryException<UsuarioActiveDirectory>> ObterUsuarioActiveDirectoryPorId(Guid usuarioAd, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var usersAD = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, new List<Guid>() { usuarioAd });

            if (usersAD.IsFailure)
                return usersAD.Failure;

            var usuariosAd = usersAD.Success;

            return usuariosAd.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuariosPorGuids(IEnumerable<Guid> activeDirectoryIds, CancellationToken cancellationToken)
        {
            var usuariosPerfisDb = await _perfilRepository.ListarPerfisUsuariosPorGuids(activeDirectoryIds, cancellationToken);

            if (usuariosPerfisDb.IsFailure)
                return usuariosPerfisDb.Failure;

            return usuariosPerfisDb;
        }

        public async Task<TryException<UsuarioModel>> ListarPerfisUsuarioPorGuid(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var usuariosPerfisDb = await _perfilRepository.ListarPerfisUsuarioPorGuid(activeDirectoryId, cancellationToken);

            if (usuariosPerfisDb.IsFailure)
                return usuariosPerfisDb.Failure;

            return usuariosPerfisDb;
        }

        public async Task<TryException<UsuarioModel>> AutenticarUsuarioPorNomeUsuario(string nomeUsuario, CancellationToken cancellationToken)
        {
            var usuarioAd = _activeDirectoryExternalService.AutenticarUsuarioPorNomeUsuario(nomeUsuario);

            if (usuarioAd.IsFailure)
                return usuarioAd.Failure;

            var usuario = usuarioAd.Success;
            
            var usuarioPerfisDb = await ObterPermissoesUsuarioOuAdicionarUsuarioPorActiveDirectoryId(usuario.Guid, cancellationToken);

            if (usuarioPerfisDb.IsFailure)
                return usuarioPerfisDb.Failure;

            var usuarioPerfis = usuarioPerfisDb.Success;

            usuarioPerfis.Nome = usuario.Nome;
            usuarioPerfis.UsuarioDeRede = usuario.UsuarioDeRede;

            return usuarioPerfis;
        }

        private async Task<TryException<UsuarioModel>> ObterPermissoesUsuarioOuAdicionarUsuarioPorActiveDirectoryId(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var usuarioPerfisDb = await ObterPermissoesUsuarioPorActiveDirectoryId(activeDirectoryId, cancellationToken);
            if (usuarioPerfisDb.IsFailure)
            {
                var novoUsuario = new UsuarioModel { ActiveDirectoryId = activeDirectoryId };
                usuarioPerfisDb = await _perfilRepository.SalvarPerfisUsuario(novoUsuario, cancellationToken);

                if (usuarioPerfisDb.IsFailure)
                    return usuarioPerfisDb.Failure;

                usuarioPerfisDb = await ObterPermissoesUsuarioPorActiveDirectoryId(activeDirectoryId, cancellationToken);
            }

            if (usuarioPerfisDb.IsFailure)
                return usuarioPerfisDb.Failure;

            return usuarioPerfisDb.Success;
        }

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuariosPorPerfil(Perfil perfil, CancellationToken cancellationToken)
        {

            var response  = await _perfilRepository.ListarUsuariosPorPerfil(perfil, cancellationToken);
            if (response.IsFailure)
                return response.Failure;


            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, response.Success.Select(x => x.ActiveDirectoryId).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var results = response.Success.Join(usuarios.Success,
                      d => d.ActiveDirectoryId,
            u => u.Guid,
            (mat, usu) => mat.DefinirEmail(usu.Email)
            );



            return results.ToCollection();

        }

       
    }
}



