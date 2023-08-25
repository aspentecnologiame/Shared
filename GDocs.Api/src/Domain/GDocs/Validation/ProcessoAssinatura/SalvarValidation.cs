using FluentValidation;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Domain.Validation.ProcessoAssinatura
{
    internal class SalvarValidation : FluentValidator<AssinaturaModel>
    {
        private const int TAMANHO_MIN_NUMERO = 1;
        private const int TAMANHO_MAX_NUMERO = 9;
        private const int CATEGORIA_STATUS_PAGAMENTO = 1;
        private const int CATEGORIA_ASSINATURA_COM_RETORNO = 31;
        private const int CATEGORIA_ASSINATURA_SEM_RETORNO = 32;

        private AssinaturaModel Instance { get; set; }

        public SalvarValidation(AssinaturaModel instance, List<ConfiguracaoCategoriaModel> configCategorias)
        {
            Instance = instance;

            var cfgCategoria = configCategorias.Find(w => w.Codigo == Instance.Informacoes.CategoriaId);

            var cfgCertificadoDigital = cfgCategoria?.CertificadoDigital;
            var cfgAssinaturaDocumento = cfgCategoria?.AssinaturaDocumento;

            NullEntity()
                .WithMessage("O processo de aprovação deve ser criado.");

            RuleFor(pad => pad.Informacoes.Titulo)
              .NotNull()
                  .WithMessage("Título é obrigatório.")
              .NotEmpty()
                  .WithMessage("Título é obrigatório.");

            RuleFor(pad => pad.Informacoes.Descricao)
              .NotNull()
                  .WithMessage("Descrição é obrigatório.")
              .NotEmpty()
                  .WithMessage("Descrição é obrigatório.");

            RuleFor(pad => pad.Informacoes.NomeDocumento)
              .NotNull()
                  .WithMessage("Nome do documento é obrigatório.")
              .NotEmpty()
                  .WithMessage("Nome do documento é obrigatório.");

            RuleFor(pad => pad.Informacoes.NumeroDocumento)
                .Must(n => n == null || n.Value > 0)
                  .WithMessage($"Número do documento deve ser maior que 0 (zero).")
                .Must(n => n == null || QtdDigitosEstaEntre(n.Value, TAMANHO_MIN_NUMERO, TAMANHO_MAX_NUMERO))
                  .WithMessage($"Número do documento deve conter entre {TAMANHO_MIN_NUMERO} e {TAMANHO_MAX_NUMERO} dígitos.");

            RuleFor(pad => pad.Informacoes.CategoriaId)
                .GreaterThan(0)
                    .WithMessage("Categoria é obrigatória.");

            RuleFor(pad => pad.Arquivos)
                .Must(a => a?.Count() > 0)
                  .WithMessage("É obrigatório anexar pelo menos 1 arquivo processo de aprovação.");

            RuleFor(pad => pad)
                .Must(VerificarQuantidadeDePassos())
                  .WithMessage("É necessário incluir ao menos 1 passo com aprovadores, mesmo que a DIR esteja selecionada.");

            VerificarPassos(cfgCertificadoDigital?.Habilitado, cfgAssinaturaDocumento?.Habilitado);
        }

        private void VerificarPassos(bool? certificadoDigitalHabilitado, bool? assinaturaDocumentoHabilitada)
        {
            RuleFor(pad => pad.Passos)
                .ChildRules(passos =>
                {
                    passos?.RuleForEach(passo => passo.Itens)
                        .ChildRules(item =>
                        {
                            item?.RuleFor(x => x.Usuarios)
                               .Must(u => u.Any())
                               .WithMessage(passo => $"Passo de aprovação nº {passo.Ordem} deve possuir pelo menos 1 aprovador associado.");

                            if (!certificadoDigitalHabilitado.HasValue && certificadoDigitalHabilitado.GetValueOrDefault())
                            {
                                item?.RuleFor(x => x.Usuarios)
                                    .Must(u => !u.Any(w => w.AssinarDigitalmente))
                                         .WithMessage(passo => $"Passo de aprovação nº {passo.Ordem} não deve possuir aprovador com certificado digital.");
                            }

                            item?.RuleForEach(u => u.Usuarios)
                                .SetValidator(new AssinantePassoValidation(Instance));
                        });
                });


            VerificarAssinaturaDocumento(assinaturaDocumentoHabilitada, certificadoDigitalHabilitado);
            VerificarCertificadoDigital(certificadoDigitalHabilitado, assinaturaDocumentoHabilitada);
        }

        private void VerificarAssinaturaDocumento(bool? assinaturaDocumentoHabilitada, bool? certificadoDigitalHabilitado)
        {
            if (assinaturaDocumentoHabilitada.HasValue && assinaturaDocumentoHabilitada.Value)
            {
                if (certificadoDigitalHabilitado.HasValue && certificadoDigitalHabilitado.Value)
                {
                    RuleFor(pad => pad.Passos.Itens)
                        .Must(VerificarSimultaneidadeAssinadorNoDocumentoComAssinadorDigital())
                        .WithMessage("Não é permitido informar aprovador com certificado digital, pois já foi selecionado um responsável pela aprovação do documento.");

                    RuleFor(pad => pad.Passos.Itens)
                        .Must(VerificarSeUmTipoDeAssinaturaFoiSelecionado())
                        .WithMessage("Selecione um responsável pela aprovação do documento ou um aprovador com certificado digital.");
                }
                else
                    RuleFor(pad => pad)
                        .Must(VerificarDocumentoAssinatura(1))
                        .WithMessage("É necessário informar uma pessoa como responsável pela aprovação do documento.");
            }
            else
                RuleFor(pad => pad)
                    .Must(VerificarDocumentoAssinatura(0))
                    .WithMessage("Categoria do documento não permite informar responsável pela aprovação do documento.");
        }

        private void VerificarCertificadoDigital(bool? certificadoDigitalHabilitado, bool? assinaturaDocumentoHabilitada)
        {
            if (certificadoDigitalHabilitado.HasValue && certificadoDigitalHabilitado.Value)
            {
                if (assinaturaDocumentoHabilitada.HasValue && assinaturaDocumentoHabilitada.Value)
                {
                    RuleFor(pad => pad.Passos.Itens)
                        .Must(VerificarSimultaneidadeAssinadorDigitalComAssinatura())
                        .WithMessage("Não é permitido informar um responsável pela aprovação do documento, pois existe assinante com certificado digital.");

                    RuleFor(pad => pad.Passos.Itens)
                        .Must(VerificarSeUmTipoDeAssinaturaFoiSelecionado())
                        .WithMessage("Selecione um responsável pela aprovação do documento ou um aprovador com certificado digital.");
                }
                else
                    RuleFor(pad => pad)
                        .Must(VerificarCertificadoDigitalDocumentoQuantidadeMinima(1))
                        .WithMessage("É necessário informar uma pessoa para aprovar digitalmente.");
            }
            else
                RuleFor(pad => pad)
                    .Must(VerificarCertificadoDigitalDocumento(0))
                    .WithMessage("Categoria do documento não permite informar aprovador com certificado digital.");
        }

        private Func<IList<AssinaturaPassoItemModel>, bool> VerificarSimultaneidadeAssinadorNoDocumentoComAssinadorDigital() =>
            Passo =>
            {
                var existeAssinatura = Passo.SelectMany(item => item.Usuarios).Any(usuario => usuario.AssinarFisicamente);

                if (!existeAssinatura)
                    return true;

                return Passo.SelectMany(item => item.Usuarios).Empty(usuario => usuario.AssinarDigitalmente);
            };

        private static Func<IList<AssinaturaPassoItemModel>, bool> VerificarSimultaneidadeAssinadorDigitalComAssinatura() =>
            Passo =>
            {
                var existeAssinadorDigital = Passo.SelectMany(item => item.Usuarios).Any(usuario => usuario.AssinarDigitalmente);

                if (!existeAssinadorDigital)
                    return true;

                return Passo.SelectMany(item => item.Usuarios).Empty(usuario => usuario.AssinarFisicamente);
            };

        private static Func<AssinaturaModel, bool> VerificarQuantidadeDePassos() =>
            pad =>
            {
                int[] categoriasValidas = { CATEGORIA_STATUS_PAGAMENTO, CATEGORIA_ASSINATURA_COM_RETORNO, CATEGORIA_ASSINATURA_SEM_RETORNO };

                if (pad.Passos.AdicionarDir && categoriasValidas.Contains(pad.Informacoes.CategoriaId))
                    return true;
                
                return (pad.Passos.AdicionarDir && pad.Passos.Itens?.Count() > 1) || (!pad.Passos.AdicionarDir && pad.Passos.Itens?.Count() > 0);
            };

        private static Func<AssinaturaModel, bool> VerificarDocumentoAssinatura(int qtdePermitida) =>
            pad =>
            {
                return (pad.Passos.Itens.SelectMany(s => s.Usuarios).Count(w => w.AssinarFisicamente) == qtdePermitida);
            };

        private static Func<AssinaturaModel, bool> VerificarCertificadoDigitalDocumentoQuantidadeMinima(int quantidadeMinima) =>
            pad =>
            {
                return (pad.Passos.Itens.SelectMany(item => item.Usuarios).Count(usuario => usuario.AssinarDigitalmente) >= quantidadeMinima);
            };

        private static Func<IList<AssinaturaPassoItemModel>, bool> VerificarSeUmTipoDeAssinaturaFoiSelecionado() =>
            Passo =>
            {
                var existeAssinatura = Passo.SelectMany(item => item.Usuarios).Any(usuario => usuario.AssinarFisicamente);
                var existeAssinadorDigital = Passo.SelectMany(item => item.Usuarios).Any(usuario => usuario.AssinarDigitalmente);

                if (!existeAssinatura && !existeAssinadorDigital)
                    return false;

                return true;
            };

        private static Func<AssinaturaModel, bool> VerificarCertificadoDigitalDocumento(int quantidadeEsperada) =>
            pad =>
            {
                return (pad.Passos.Itens.SelectMany(item => item.Usuarios).Count(usuario => usuario.AssinarDigitalmente) == quantidadeEsperada);
            };

        private bool QtdDigitosEstaEntre(int num, int min, int max)
        {
            int lenght = num.ToString().Length;
            return lenght >= min && lenght <= max;
        }
    }
}