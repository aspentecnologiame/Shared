using FluentValidation;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ICE.GDocs.Domain.GDocs.Validation.SaidaMaterialNotaFiscal
{
    internal class CadastroValidation : FluentValidator<SaidaMaterialNotaFiscalModel>
    {
        private const string ADICIONAR_CODIGOERRO = "SaidaMaterialNF:acao:adiciona";
        private const int CAMPO_TAMANHO_MIN = 3;
        private const int CAMPO_TAMANHO_MAX = 25;
        private const int CAMPO_TAMANHO_MAX_DESTINO = 60;
        private const int CAMPO_TANSPORTADOR_TAMANHO_MAX = 35;
        private readonly  DateTime DataAtual = DateTime.Today;
        private const int TAMANHO_MAX = 255;
        private const int TAMANHO_MAX_TOTVS = 32;
        private const int CNPJ_MAX = 14;
        private const int CPF_MAX = 11;

        private const int REFERENCIA_TAMANHO_MIN = 3;
        private const int REFERENCIA_TAMANHO_MAX = 100;
        public CadastroValidation(SaidaMaterialNotaFiscalModel model)
        {
            NullEntity()
            .WithMessage("O documento Saida de material com nota fiscal deve ser informado.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.Saida)
            .NotNull()
            .WithMessage("A Data de Saída é obrigatoria.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .GreaterThan(DataAtual)
            .WithMessage("A Data de Saída nao pode ser menor que a data atual.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            if (model.FlgRetorno)
            {
             RuleFor(doc => doc.Retorno)
             .NotNull()
             .WithMessage("A Previsão de retorno é obrigatoria.")
             .WithErrorCode(ADICIONAR_CODIGOERRO)
             .GreaterThan(DataAtual)
             .WithMessage("A Previsão de retorno não pode ser menor que a data atual.")
             .WithErrorCode(ADICIONAR_CODIGOERRO);
            }


            RuleFor(doc => doc.SetorResponsavel)
            .NotEmpty()
            .WithMessage("O Setor é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .Length(CAMPO_TAMANHO_MIN, CAMPO_TAMANHO_MIN)
            .WithMessage($"O Setor deve conter  {CAMPO_TAMANHO_MIN} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(doc => doc.Origem)
            .NotEmpty()
            .WithMessage("A Origem é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .MaximumLength(CAMPO_TAMANHO_MAX)
            .WithMessage($"A Origem deve conter entre {REFERENCIA_TAMANHO_MIN} e {REFERENCIA_TAMANHO_MAX} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            RuleFor(doc => doc.Destino)
            .NotEmpty()
            .WithMessage("O Destino é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO)
            .MaximumLength(CAMPO_TAMANHO_MAX_DESTINO)
            .WithMessage($"O Destino  deve conter entre {REFERENCIA_TAMANHO_MIN} e {REFERENCIA_TAMANHO_MAX} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);

            this.ValidationCadastro(model);
           
        }

        private void ValidationCadastro(SaidaMaterialNotaFiscalModel model)
        {
            RuleFor(doc => doc.Transportador)
                       .NotEmpty()
                       .WithMessage("O campo Quem vai trasportar é obrigatório.")
                       .WithErrorCode(ADICIONAR_CODIGOERRO)
                       .MaximumLength(CAMPO_TANSPORTADOR_TAMANHO_MAX)
                       .WithMessage($"O campo Quem vai trasportar  deve conter entre {REFERENCIA_TAMANHO_MIN} e {CAMPO_TANSPORTADOR_TAMANHO_MAX} caracteres.")
                       .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(doc => doc.Peso)
            .NotEmpty()
            .WithMessage("O Peso é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(doc => doc.Volume)
            .NotEmpty()
            .WithMessage("O Volume é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(doc => doc.NaturezaOperacional)
            .NotEmpty()
            .WithMessage("O Natureza da operação é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(doc => doc.ModalidadeFrete)
            .NotEmpty()
            .WithMessage("O Modalidade Frete é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            RuleFor(doc => doc.Motivo)
            .NotEmpty()
            .WithMessage("O Motivo é obrigatório.")
            .MaximumLength(TAMANHO_MAX)
            .WithMessage($"O Motivo deve conter no maximo. {TAMANHO_MAX} caracteres");

            if (model.Documento != null && model.Documento.Length <= CPF_MAX)
            {
                RuleFor(doc => doc.Documento)
               .MinimumLength(CPF_MAX)
               .WithMessage($"O CPF deve conter {CPF_MAX} caracteres.")
               .MaximumLength(CPF_MAX)
               .WithMessage($"O CPF deve conter no maximo {CPF_MAX} caracteres.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);
            }
            else 
            {
                RuleFor(doc => doc.Documento)
                .MinimumLength(CNPJ_MAX)
                .WithMessage($"O CNPJ deve conter {CNPJ_MAX} caracteres.")
                .MaximumLength(CNPJ_MAX)
                .WithMessage($"O CNPJ deve conter no maximo {CNPJ_MAX} caracteres.")
                .WithErrorCode(ADICIONAR_CODIGOERRO);
            }


            RuleFor(doc => doc.ItemMaterialNf)
            .Must(x => x.Any())
            .WithMessage($"Obrigatório adicionar ao menos um material.")
            .WithErrorCode(ADICIONAR_CODIGOERRO);


            if ((model.Fornecedor == null))
            {
                RuleFor(doc => doc.CodigoTotvs)
               .NotEmpty()
               .WithMessage("O CodigoTotvs é obrigatório.")
               .WithErrorCode(ADICIONAR_CODIGOERRO)
               .MaximumLength(TAMANHO_MAX_TOTVS)
               .WithMessage($"O campo Codigo Totvs deve conter entre {REFERENCIA_TAMANHO_MIN} e {TAMANHO_MAX_TOTVS} caracteres.")
               .WithErrorCode(ADICIONAR_CODIGOERRO);
            }
        }

    }
}
