using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class PassoUsuarioDownload
    {
        public int PadId { get; set; }
        public Guid UsuarioId { get; set; }
        public string NomeUsuario { get; set; }
        
    }
}
