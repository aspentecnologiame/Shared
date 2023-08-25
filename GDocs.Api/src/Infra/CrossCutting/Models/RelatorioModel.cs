using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class RelatorioModel
    {
        public int IdRelatorio { get; set; }
        public int IdTipoNotificacao { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Parametros { get; set; } = string.Empty;
    }
}
