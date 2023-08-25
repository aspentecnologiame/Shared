using System.ComponentModel;

namespace ICE.GDocs.Infra.CrossCutting.Models.Enums
{
    public enum SolicitacaoSaidaMaterialStatus
    {
        [Description("Em construção")]
        EmConstrucao = 0,
        [Description("Em Aprovação")]
        PendenteAprovacao = 1,
        [Description("Saída Pendente")]
        PendenteSaida = 2,
        [Description("Reprovado")]
        Reprovado = 3,
        [Description("Cancelado")]
        Cancelado = 4,
        [Description("Em Aberto")]
        EmAberto = 5,
        [Description("Concluída")]
        Concluido = 6,
        [Description("Em Aprovação de Ciência")]
        AprovacaoCiencia = 7
    }
}
