using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums
{
    public enum SaidaMaterialNotaFiscalStatus
    {
        [Description("Pendente NF Saída")]
        PendenteNFSaida = 1,

        [Description("Saída Pendente")]
        PendenteSaida = 2,

        [Description("Em aberto")]
        EmAberto = 3,
        
        [Description("Pendente NF Retorno")]
        PendenteNFRetorno = 4,

        [Description("Em aprovação de ciência")]
        EmAprovaçãoDeciencia = 5,

        [Description("Pendente NF Cancelamento")]
        PendenteNFCancelamento = 6,

        [Description("Cancelada")]
        Cancelada = 7,

        [Description("Concluída")]
        Concluida = 8,

    }
}
