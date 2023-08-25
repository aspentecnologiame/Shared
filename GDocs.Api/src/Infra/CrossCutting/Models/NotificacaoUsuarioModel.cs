using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class NotificacaoUsuarioModel : BaseModel
    {
        public int IdNotificacaoUsuario { get; set; }
        public int IdTipoNotificacao { get; set; }
        public Guid IdUsuario { get; set; }
        public bool Lido { get; set; }
        public DateTime DataLeitura { get; set; }
    }
}
