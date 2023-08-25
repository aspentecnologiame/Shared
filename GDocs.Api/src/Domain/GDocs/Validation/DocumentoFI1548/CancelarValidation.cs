using FluentValidation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;

namespace ICE.GDocs.Domain.Validation.DocumentoFI1548
{
    internal class CancelarValidation : FluentValidator<DocumentoFI1548Model>
    {
        private const string CANCELAR_CODIGOERRO = "documentofi1548:cancelar";

        public CancelarValidation(Guid ActiveDirectoryId, bool permiteCancelarTodos)
        {
            NullEntity()
                .WithMessage("O documento deve ser informado.")
                .OverridePropertyName(CANCELAR_CODIGOERRO);

            RuleFor(x => x.AutorId)
                .Must(au => au == ActiveDirectoryId || permiteCancelarTodos)
                .OverridePropertyName("Usuário não possui permissão para cancelar documento.");

        }
    }
}
