using FluentValidation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;

namespace ICE.GDocs.Domain.Validation.SolicitacaoSaidaMaterial
{
    internal class CancelarValidation : FluentValidator<SolicitacaoSaidaMaterialModel>
    {
        private const string CANCELAR_CODERRO = "documentofi347:cancelar";

        public CancelarValidation(Guid ActiveDirectoryId, bool permiteCancelarTodos)
        {
            NullEntity()
                .WithMessage("O usuário deve ser informado.")
                .OverridePropertyName(CANCELAR_CODERRO);

            RuleFor(x => x.GuidResponsavel)
                .Must(au => au == ActiveDirectoryId || permiteCancelarTodos)
                .OverridePropertyName("Usuário não possui permissão para cancelar solicitação de saída de material.");
        }
    }
}
