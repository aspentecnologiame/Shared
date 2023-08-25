using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class TipoNotificacaoModel : BaseModel
    {
        public int IdTipoNotificacao { get; set; }
        public string CodigoAcao { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; 
    }
}
