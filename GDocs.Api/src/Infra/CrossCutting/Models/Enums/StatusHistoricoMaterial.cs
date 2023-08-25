using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.Enums
{
    public enum StatusHistoricoMaterial
    {
        [Description("Pendente de Saída")]
        PendenteSaida,

        [Description("Pendente de Retorno")]
        PendenteRetorno,

        [Description("Baixa sem retorno")]
        BaixaSemRetorno,

        [Description("Retornado")]
        Retornado,

        [Description("Concluído")]
        Concluido,
        
    }
}
