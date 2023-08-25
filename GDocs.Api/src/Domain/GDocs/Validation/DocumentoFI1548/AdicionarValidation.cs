using FluentValidation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Domain.Validation.DocumentoFI1548
{
    internal class AdicionarValidation : FluentValidator<DocumentoFI1548Model>
    {
        private const string ADICIONAR_CODIGOERRO = "documentofi1548:adicionar";
        private const int DESCRICAO_TAMANHO_MIN = 3;
        private const int DESCRICAO_TAMANHO_MAX = 640;
        private const int REFERENCIA_TAMANHO_MIN = 3;
        private const int REFERENCIA_TAMANHO_MAX = 100;
        private const decimal VALOR_MIN = 0.0M;
        private List<string> TiposPagamento { get; set; }
        private List<string> Fornecedores { get; set; }

        public AdicionarValidation(DocumentoFI1548Model instance)
        {
            TiposPagamento = new List<string> { "Unico", "Parcial", "Parcelado" };
            Fornecedores = new List<string> { "Unico", "Diversos" };

            NullEntity()
                .WithMessage("O documento deve ser informado.")
                .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.Descricao)
               .NotNull()
               .WithMessage("Descrição é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .NotEmpty()
               .WithMessage("Descrição é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .Length(DESCRICAO_TAMANHO_MIN, DESCRICAO_TAMANHO_MAX)
               .WithMessage($"Descrição deve conter entre {DESCRICAO_TAMANHO_MIN} e {DESCRICAO_TAMANHO_MAX} caracteres.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.Referencia)
               .NotNull()
               .WithMessage("Referência é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .NotEmpty()
               .WithMessage("Referência é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .Length(REFERENCIA_TAMANHO_MIN, REFERENCIA_TAMANHO_MAX)
               .WithMessage($"Referência deve conter entre {REFERENCIA_TAMANHO_MIN} e {REFERENCIA_TAMANHO_MAX} caracteres.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.Valor)
               .GreaterThan(VALOR_MIN)
               .WithMessage($"Valor deve ser maior que é {VALOR_MIN:C2}.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.TipoPagamento)
               .NotNull()
               .WithMessage("Tipo de Pagamento é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .NotEmpty()
               .WithMessage("Tipo de Pagamento é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .Must(tp => TiposPagamento.Select(t => t.ToLowerInvariant()).Contains(tp.ToLowerInvariant()))
               .WithMessage("Tipo de pagamento inválido.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .Must(tp => tp.ToLowerInvariant() != "parcelado" || instance.QuantidadeParcelas > 0)
               .WithMessage("Para pagamentos parcelados, pelo menos uma parcela é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.Fornecedor)
               .NotNull()
               .WithMessage("Fornecedor é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .NotEmpty()
               .WithMessage("Fornecedor é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .Must(tp => Fornecedores.Select(t => t.ToLowerInvariant()).Contains(tp.ToLowerInvariant()))
               .WithMessage("Fornecedor inválido.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);
        }
    }
}
