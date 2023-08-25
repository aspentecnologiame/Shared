using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class NotificacaoRelatorioModel : BaseModel
    {
        public int IdNotificacaoUsuario { get; set; }
        public int IdRelatorio { get; set; }
        public Guid IdUsuario { get; set; }
        public bool Lido { get; set; }
        public DateTime DataLeitura { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Parametros { get; set; } = string.Empty;
        public byte[] RdlBytes { get; set; }
    }
}
