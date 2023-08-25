using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace ICE.GDocs.Infra.ExternalServices.ActiveDirectory
{
    internal class ActiveDirectoryExternalService : IActiveDirectoryExternalService
    {
        private readonly string _dominio;
        private readonly int _diasValidadeDaSenha;
        private const int DIAS_VALIDADE_DA_SENHA_PADRAO = 90;

        public ActiveDirectoryExternalService(IConfiguration configuration)
        {
            _dominio = configuration.GetValue<string>("Infra:ExternalServices:ActiveDirectory:Dominio");
            _diasValidadeDaSenha = configuration.GetValue("Infra:ExternalServices:ActiveDirectory:DiasValidadeDaSenha", DIAS_VALIDADE_DA_SENHA_PADRAO);
        }

        public TryException<IEnumerable<UsuarioActiveDirectory>> GetActiveDirectoryUsers(string filterName, List<Guid> filterGuids)
        {
            if (string.IsNullOrEmpty(filterName) && filterGuids.Empty())
                return new List<UsuarioActiveDirectory>();


            if (filterGuids.Any())
            {
                return GetByGuidsAndName(filterName, filterGuids);
            }

            return GetByName(filterName);

        }

        public TryException<UsuarioActiveDirectory> AutenticarUsuario(string nomeUsuario, string senha)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, _dominio))
            {
                var acessoValido = ctx.ValidateCredentials(nomeUsuario, senha);

                /// Caso o usuário esteja inativo, a credencial não é validada pelo AD
                if (!acessoValido)
                    return new BusinessException("usuario-senha-invalidos", "Usuário ou senha inválidos.");

                var userAD = UserPrincipal.FindByIdentity(ctx, nomeUsuario);

                if (userAD.AccountExpirationDate.HasValue && userAD.AccountExpirationDate < DateTime.Now)
                    return new BusinessException("usuario-conta-expirado", "Conta expirada no AD.");

                if (!userAD.PasswordNeverExpires && userAD.LastPasswordSet.Value.AddDays(_diasValidadeDaSenha) < DateTime.Now)
                    return new BusinessException("usuario-senha-expirada", "Senha expirada no AD.");

                return new UsuarioActiveDirectory(userAD.Guid.Value, userAD.DisplayName, userAD.UserPrincipalName, userAD.Description, userAD.SamAccountName);
            }
        }

        private TryException<IEnumerable<UsuarioActiveDirectory>> GetByGuidsAndName(string filterName, List<Guid> filterGuids)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, _dominio))
            {
                var users = new List<UsuarioActiveDirectory>();

                foreach (var guid in filterGuids)
                {
                    var userAD = UserPrincipal.FindByIdentity(ctx, guid.ToString());

                    var usuarioHabilitado = userAD?.Enabled ?? false;

                    if (!usuarioHabilitado)
                        continue;

                    if (string.IsNullOrEmpty(filterName) || userAD.DisplayName.ToUpperInvariant().Contains(filterName.ToUpperInvariant()))
                        users.Add(new UsuarioActiveDirectory(userAD.Guid.Value, userAD.DisplayName, userAD.UserPrincipalName, userAD.Description, userAD.SamAccountName));
                }

                return users;
            }
        }

        private TryException<IEnumerable<UsuarioActiveDirectory>> GetByName(string filterName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, _dominio))
            {
                var users = new List<UsuarioActiveDirectory>();

                var userPrinciple = new UserPrincipal(ctx)
                {
                    Enabled = true,
                    Surname = "*",
                    DisplayName = $"{filterName}*"
                };

                using (var search = new PrincipalSearcher(userPrinciple))
                {
                    foreach (var userAD in search.FindAll())
                    {
                        if (userAD.DisplayName != null && userAD.Guid != null)
                            users.Add(new UsuarioActiveDirectory(userAD.Guid.Value, userAD.DisplayName, userAD.UserPrincipalName, userAD.Description, userAD.SamAccountName));
                    }
                }

                return users;
            }
        }

        public TryException<UsuarioActiveDirectory> AutenticarUsuarioPorNomeUsuario(string nomeUsuario)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, _dominio))
            {
                var userAD = UserPrincipal.FindByIdentity(ctx, nomeUsuario);
                return new UsuarioActiveDirectory(userAD.Guid.Value, userAD.DisplayName, userAD.UserPrincipalName, userAD.Description, userAD.SamAccountName);
            }
        }
    }
}

