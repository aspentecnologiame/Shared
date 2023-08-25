using ICE.GDocs.Domain.ExternalServices.Model;
using System;
using System.Collections.Generic;

namespace ICE.GDocs.Domain.ExternalServices
{
    public interface IActiveDirectoryExternalService
    {
        TryException<IEnumerable<UsuarioActiveDirectory>> GetActiveDirectoryUsers(string filterName, List<Guid> filterGuids);
        TryException<UsuarioActiveDirectory> AutenticarUsuario(string nomeUsuario, string senha);
        TryException<UsuarioActiveDirectory> AutenticarUsuarioPorNomeUsuario(string nomeUsuario);
    }
}
