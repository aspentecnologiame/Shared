using FluentValidation;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.GDocs.Validation.SolicitacaoSaidaMaterial
{
    internal class CadastroItemValidator :FluentValidator<SolicitacaoSaidaMaterialItemModel>
    {
        private const string ADD_CODIGO_ERRO = "fi347:adicionar";
        private const int CAMPO_TAMANHO_MIN_QUANT = 0;
        private const int TAMANHO_DESC_MAX = 150;
        private const int TAMANHO_UNIDADE_MAX = 50;
        private const int TAMANHO_PATRI_MAX = 50;

        public CadastroItemValidator()
        {
            NullEntity()
            .WithMessage("Obrigatório adicionar ao menos um material.")
            .WithErrorCode(ADD_CODIGO_ERRO);

            RuleFor(i=> i.Quantidade)
            .GreaterThan(CAMPO_TAMANHO_MIN_QUANT)
            .WithMessage("O Quantidade é obrigatório.");

            RuleFor(i=> i.Unidade)
            .NotEmpty()
            .WithMessage("O Unidade é obrigatório.")
            .WithErrorCode(ADD_CODIGO_ERRO)
            .MaximumLength(TAMANHO_UNIDADE_MAX)
            .WithMessage($"O Unidade deve conter no maxímo {TAMANHO_UNIDADE_MAX} caracteres.")
            .WithErrorCode(ADD_CODIGO_ERRO);

            RuleFor(i=> i.Descricao)
           .NotEmpty()
           .WithMessage("A Descrição é obrigatório.")
           .WithErrorCode(ADD_CODIGO_ERRO)
           .MaximumLength(TAMANHO_DESC_MAX)
           .WithMessage($"A Descrição deve conter no maxímo {TAMANHO_DESC_MAX} caracteres.")
           .WithErrorCode(ADD_CODIGO_ERRO);

            RuleFor(i=> i.Patrimonio)
           .MaximumLength(TAMANHO_PATRI_MAX)
           .WithMessage($"O Patrimonio deve conter no maxímo {TAMANHO_PATRI_MAX} caracteres.")
           .WithErrorCode(ADD_CODIGO_ERRO);

        }
    }
}
