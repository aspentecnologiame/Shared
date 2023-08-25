using FluentValidation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Domain.Validation.ProcessoAssinatura
{
    internal class AssinantePassoValidation : FluentValidator<AssinaturaUsuarioModel>
    {
        private AssinaturaModel Instance { get; set; }

        public AssinantePassoValidation(AssinaturaModel instance)
        {
            Instance = instance;

            RuleFor(u => u.Guid)
                .Must(guid => ObterGuidAssinanteDuplicadoList() == null || !ObterGuidAssinanteDuplicadoList().Contains(guid))
                            .WithMessage(u => $"Assinante {u.Nome} deve estar associado somente à 1 passo de assinatura.");
        }

        private List<Guid> ObterGuidAssinanteDuplicadoList()
            => Instance.Passos.Itens
                .SelectMany(u => u.Usuarios)?
                .GroupBy(x => x.Guid)?
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key)?
                        .ToList();
    }
}
