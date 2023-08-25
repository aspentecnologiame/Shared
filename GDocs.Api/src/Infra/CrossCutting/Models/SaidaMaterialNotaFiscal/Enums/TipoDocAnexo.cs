using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums
{
    public enum TipoDocAnexo
    {
        [Description("Nota fiscal de saída")]
        Saida = 1,

        [Description("Nota fiscal de retorno")]
        Retorno = 2,
    }
}
