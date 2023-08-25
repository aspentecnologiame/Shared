using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models.Enums
{
    public enum SaidaMaterialTipoAcao
    {
        [Description("Registro de saída")]
        RegistroSaida = 1,
        [Description("Registro de retorno")]
        RegistroRetorno = 2,
        [Description("Solicitação de prorrogação")]
        SolicitacaoProrrogacao = 3,
        [Description("Solicitação de cancelamento")]
        SolicitacaoCancelamento = 4,
        [Description("Baixa de material sem retorno")]
        BaixaMaterialSemRetorno = 5,
        [Description("Solicitacao Baixa de material sem retorno")]
        SolicitacaoBaixaSemRetorno = 6
    }
}
