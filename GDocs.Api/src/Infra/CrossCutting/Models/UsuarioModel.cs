using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class UsuarioModel
    {
        public string Nome { get; set; }
        public string UsuarioDeRede { get; set; }
        public string Email { get; set; }
        public Guid ActiveDirectoryId { get; set; }
        public IEnumerable<PerfilModel> Perfis { get; set; }
        public bool Status { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public IEnumerable<FuncionalidadeModel> Permissoes { get; set; }

        public UsuarioModel()
        {
            Perfis = new List<PerfilModel>();
            Permissoes = new List<FuncionalidadeModel>();
        }

        public UsuarioModel DefinirEmail(string email)
        {
            this.Email = email;
            return this;
        }
    }
}
