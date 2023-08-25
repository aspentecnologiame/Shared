using System;

namespace ICE.GDocs.Domain.ExternalServices.Model
{
    public class UsuarioActiveDirectory
    {
        public Guid Guid { get; }
        public string Nome { get; }
        public string Email { get; }
        public string Descricao { get; }
        public string UsuarioDeRede { get; }


        public UsuarioActiveDirectory
            (
                Guid guid,
                string displayName,
                string email,
                string description,
                string usuarioDeRede
            )
        {
            Guid = guid;
            Nome = displayName;
            Email = email;
            Descricao = description;
            UsuarioDeRede = usuarioDeRede;
        }
    }
}