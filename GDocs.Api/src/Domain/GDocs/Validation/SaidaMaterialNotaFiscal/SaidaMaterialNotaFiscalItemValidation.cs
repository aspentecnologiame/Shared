using FluentValidation;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.GDocs.Validation.SaidaMaterialNotaFiscal
{
    internal class SaidaMaterialNotaFiscalItemValidation : FluentValidator<SaidaMaterialNotaFiscalItemModel>
    {

        private const string ADICIONAR_CODIGOERRO = "SaidaMaterialNF:acao:adiciona";
        private const int TAMANHO_MAX_MEDIO = 64;
        private const int CAMPO_INT_TAMANHO_MIN = 0;
        private const int TAMANHO_MAX_DESC = 150;

        private const int TAMANHO_MAX_UNIDADE = 50;
        private const int TAMANHO_MAX_CODIGO = 10;
        public SaidaMaterialNotaFiscalItemValidation(SaidaMaterialNotaFiscalItemModel item)
        {
            NullEntity()
            .WithMessage("Obrigatório adicionar ao menos um material.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(item => item.Quantidade)
            .GreaterThan(CAMPO_INT_TAMANHO_MIN)
            .WithMessage("O Quantidade é obrigatório.");

            RuleFor(item => item.Unidade)
            .NotEmpty()
            .WithMessage("O Unidade é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .MaximumLength(TAMANHO_MAX_UNIDADE)
            .WithMessage($"O Unidade deve conter no maxímo {TAMANHO_MAX_UNIDADE} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(item => item.Descricao)
           .NotEmpty()
           .WithMessage("A Descrição é obrigatório.")
           .WithErrorCode(ADICIONAR_CODIGOERRO)
           .MaximumLength(TAMANHO_MAX_DESC)
           .WithMessage($"A Descrição deve conter no maxímo {TAMANHO_MAX_MEDIO} caracteres.")
           .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(item => item.Codigo)
           .NotEmpty()
           .WithMessage("A Codigo é obrigatório.")
           .WithErrorCode(ADICIONAR_CODIGOERRO)
           .MaximumLength(TAMANHO_MAX_CODIGO)
           .WithMessage($"A Codigo deve conter no maxímo {TAMANHO_MAX_CODIGO} caracteres.")
           .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(item => item.TagService)
           .MaximumLength(TAMANHO_MAX_CODIGO)
           .WithMessage($"A Tag Service deve conter no maxímo {TAMANHO_MAX_CODIGO} caracteres.")
           .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(item => item.Patrimonio)
           .MaximumLength(TAMANHO_MAX_CODIGO)
           .WithMessage($"O Patrimonio deve conter no maxímo {TAMANHO_MAX_CODIGO} caracteres.")
           .WithErrorCode(ADICIONAR_CODIGOERRO);

        }
    }
}
