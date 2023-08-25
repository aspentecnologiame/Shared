using FluentValidation;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.GDocs.Validation.SaidaMaterialNotaFiscal
{
    internal class FornecedorValidator : FluentValidator<Fornecedor>
    {
        private const string ADICIONAR_CODIGOERRO = "SaidaMaterialNF:acao:adiciona";
        private const int TAMANHO_MAX = 255;
        private const int TAMANHO_MAX_MEDIO = 64;
        private const int TAMANHO_MAX_CEP = 9;
        private const int CAMPO_TAMANHO_MIN = 3;
        private const int TAMANHO_MAX_ESTADO = 2;

        public FornecedorValidator(Fornecedor fornecedor)
        {

            NullEntity()
            .WithMessage("O Fornecedor deve ser informado.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(f => f.Endereco)
            .NotEmpty()
            .WithMessage("O Endereço é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .Length(CAMPO_TAMANHO_MIN, TAMANHO_MAX)
            .WithMessage($"O Endereço deve conter  {CAMPO_TAMANHO_MIN} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(f => f.Bairro)
            .NotEmpty()
            .WithMessage("O Bairro é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .Length(CAMPO_TAMANHO_MIN, TAMANHO_MAX_MEDIO)
            .WithMessage($"O Bairro deve conter  {CAMPO_TAMANHO_MIN} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(f => f.Cep)
            .NotEmpty()
            .WithMessage("O Cep é obrigatório.")
            .MaximumLength(TAMANHO_MAX_CEP)
            .WithMessage($"O campo Cep o maxímo permitido é de {TAMANHO_MAX_CEP} caracteres.");

            RuleFor(f => f.Cidade)
            .NotEmpty()
            .WithMessage("O Cidade é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .Length(CAMPO_TAMANHO_MIN, TAMANHO_MAX_MEDIO)
            .WithMessage($"O Cidade deve conter  {CAMPO_TAMANHO_MIN} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(f => f.Estado)
            .NotEmpty()
            .WithMessage("O Estado é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .Length(TAMANHO_MAX_ESTADO)
            .WithMessage($"O Estado deve conter  {TAMANHO_MAX_ESTADO} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

        }
    }
}
