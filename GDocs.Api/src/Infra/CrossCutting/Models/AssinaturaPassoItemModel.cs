using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaPassoItemModel
    {
        public int Id { get; set; }
        public int Ordem { get; set; }
        public bool AguardarTodosUsuarios { get; set; }
        public IList<AssinaturaUsuarioModel> Usuarios { get; private set; } = new List<AssinaturaUsuarioModel>();
        public StatusAssinaturaDocumentoPasso Status { get; set; }
        public string DescricaoStatus { get; set; }

        public AssinaturaPassoItemModel()
        {
            Status = StatusAssinaturaDocumentoPasso.NaoIniciado;
        }

        public void DefinirUsuarios(IList<AssinaturaUsuarioModel> usuarios)
         => this.Usuarios = usuarios;

        public void DefinirUsuario(AssinaturaUsuarioModel usuario)
     => this.Usuarios.Add(usuario);
    }
}
