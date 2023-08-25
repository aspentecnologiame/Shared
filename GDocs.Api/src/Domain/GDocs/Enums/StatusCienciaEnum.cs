using System.ComponentModel;

namespace ICE.GDocs.Domain.GDocs.Enums
{
    public enum StatusCiencia
    {
      
        [Description("Pendente")]
        Pendente = 1,

        [Description("Concluído")]
        Concluido = 2,

        [Description("Rejeitado")]
        Rejeitado = 3,
    }
}
