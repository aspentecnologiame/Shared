using FluentValidation;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Domain.Validation.DocumentoFI1548
{
    internal class LiquidarValidation : FluentValidator<DocumentoFI1548Model>
    {
        private const string LIQUIDAR_CODIGOERRO = "documentofi1548:liquidar";

        public LiquidarValidation(bool permiteLiquidar)
        {
            NullEntity()
                .WithMessage("O documento deve ser informado.")
                .WithErrorCode(LIQUIDAR_CODIGOERRO);

            RuleFor(x => x.DataLiquidacao)
                .Must(_ => permiteLiquidar)
                .WithMessage("Usuário não possui permissão para liquidar o documento.")
                .WithErrorCode(LIQUIDAR_CODIGOERRO);

            RuleFor(s => s.Status)
                .Equal(DocumentoFI1548Status.EmAberto)
                .WithMessage("Documento não pode ser liquidado.")
                .WithErrorCode(LIQUIDAR_CODIGOERRO);
        }
    }
}
