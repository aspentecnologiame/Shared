using FluentValidation;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System.Linq;
using System;

namespace ICE.GDocs.Domain.GDocs.Validation.SolicitacaoSaidaMaterial
{
    internal class CadastroValidation : FluentValidator<SolicitacaoSaidaMaterialModel>
    {
        private const string ADICIONAR_CODIGO_ERRO = "fi347:adicionar";
        private const int CAMPO_TAMANHO_MIN = 3;
        private readonly DateTime DataAtual = DateTime.Today;
        private const int TAMANHO_MAX = 255;
        private const int CAMPO_TAMANHO_MAX_ORIGEM = 10;
        private const int CAMPO_TAMANHO_MAX_DESTINO = 70;

        public CadastroValidation(SolicitacaoSaidaMaterialModel model)
        {
            NullEntity()
            .WithMessage("O documento Saida de material deve ser informado.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO);

            RuleFor(doc => doc.SetorResponsavel)
            .NotEmpty()
            .WithMessage("O Setor é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO)
            .Length(CAMPO_TAMANHO_MIN, CAMPO_TAMANHO_MIN)
            .WithMessage($"O Setor deve conter  {CAMPO_TAMANHO_MIN} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO);


            RuleFor(doc => doc.Origem)
            .NotEmpty()
            .WithMessage("A Origem é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO)
            .MaximumLength(CAMPO_TAMANHO_MAX_ORIGEM)
            .WithMessage($"A Origem deve conter no máximo {CAMPO_TAMANHO_MAX_ORIGEM} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO);

            RuleFor(doc => doc.Destino)
            .NotEmpty()
            .WithMessage("O Destino é obrigatório.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO)
            .MaximumLength(CAMPO_TAMANHO_MAX_DESTINO)
            .WithMessage($"O Destino deve conter no máximo {CAMPO_TAMANHO_MAX_DESTINO} caracteres.")
            .WithErrorCode(ADICIONAR_CODIGO_ERRO);

            if (model.FlgRetorno)
            {
                RuleFor(doc => doc.Retorno)
                .NotNull()
                .WithMessage("A Data de prevista de retorno é obrigatoria.")
                .WithErrorCode(ADICIONAR_CODIGO_ERRO)
                .GreaterThan(DataAtual)
                .WithMessage("A Data de prevista de retorno nao pode ser menor que a data atual.")
                .WithErrorCode(ADICIONAR_CODIGO_ERRO);
            }

            RuleFor(doc => doc.Observacao)
           .MaximumLength(TAMANHO_MAX)
           .WithMessage($"A Observação deve conter no maximo. {TAMANHO_MAX} caracteres");

            RuleFor(doc => doc.Motivo)
           .NotEmpty()
           .WithMessage("O Motivo é obrigatório.")
           .WithErrorCode(ADICIONAR_CODIGO_ERRO)
           .MaximumLength(TAMANHO_MAX)
           .WithMessage($"O Motivo deve conter no maximo. {TAMANHO_MAX} caracteres");

            RuleFor(doc => doc.ItemMaterial)
           .Must(x => x.Any())
           .WithMessage($"Obrigatório adicionar ao menos um material.")
           .WithErrorCode(ADICIONAR_CODIGO_ERRO);

        }
    }
}
