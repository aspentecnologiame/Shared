using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class CienciaUsuariosProvacao
    {
        public int Id { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime? Aprovacao { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }
        public bool FlgRejeitado { get; set; }

    }
}
