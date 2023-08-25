using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Transactions;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoCiencia;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoCienciaService : DomainService, ISolicitacaoCienciaService
    {
        private readonly ISolicitacaoCienciaRepository _solicitacaoCienciaRepository;
        private readonly ISolicitacaoSaidaMaterialRepository _solicitacaoSaidaMaterialRepository;
        private readonly ISolicitacaoSaidaMaterialItemRepository _solicitacaoSaidaMaterialItemRepository;
        private readonly ISolicitacaoSaidaMaterialAcaoService _solicitacaoSaidaMaterialAcaoService;
        private readonly IEmailExternalService _emailExternalService;
        private readonly IConfiguration _configuration;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;

        public SolicitacaoCienciaService(
            IUnitOfWork unitOfWork,
            ISolicitacaoCienciaRepository solicitacaoCienciaRepository,
            ISolicitacaoSaidaMaterialRepository solicitacaoSaidaMaterialRepository,
            ISolicitacaoSaidaMaterialItemRepository solicitacaoSaidaMaterialItemRepository,
            ISolicitacaoSaidaMaterialAcaoService solicitacaoSaidaMaterialAcaoService,
            IEmailExternalService emailExternalService,
            IConfiguration configuration,
            IActiveDirectoryExternalService activeDirectoryExternalService) : base(unitOfWork)
        {
            _emailExternalService = emailExternalService;
            _solicitacaoCienciaRepository = solicitacaoCienciaRepository;
            _solicitacaoSaidaMaterialItemRepository = solicitacaoSaidaMaterialItemRepository;
            _solicitacaoSaidaMaterialRepository = solicitacaoSaidaMaterialRepository;
            _solicitacaoSaidaMaterialAcaoService = solicitacaoSaidaMaterialAcaoService;
            _configuration = configuration;
            _activeDirectoryExternalService = activeDirectoryExternalService;
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
            => await _solicitacaoCienciaRepository.ListarCienciasPendentesDeAprovacaoPeloUsuario(activeDirectoryId, cancellationToken);

        public async Task<TryException<bool>> RegistroCienciaPorUsuario(SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            TryException<bool> result = false;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var idCienciaAprovador = await _solicitacaoCienciaRepository.ObterAprovadoresPorUsuarioECiencia(solicitacaoCienciaModel.Id, usuarioModel.ActiveDirectoryId, cancellationToken);

                if (idCienciaAprovador.IsFailure)
                    return idCienciaAprovador.Failure;

                if (idCienciaAprovador.Success == null)
                  return new BusinessException( $"Não foi encontrado nenhuma aprovação pendente. Para solicitação de número: {solicitacaoCienciaModel.NumeroMaterial}");

                await _solicitacaoCienciaRepository.RegistroCiencia(solicitacaoCienciaModel, idCienciaAprovador.Success.Id, cancellationToken);

                var aprovadorCiencia = await _solicitacaoCienciaRepository.ObterAprovadoresPorCiencia(solicitacaoCienciaModel.Id, cancellationToken);

                if (aprovadorCiencia.IsFailure)
                    return aprovadorCiencia.Failure;

                result = await ValidarAprovacaoFinal(aprovadorCiencia.Success, solicitacaoCienciaModel, usuarioModel, transaction, cancellationToken);

               transaction.Commit();
            }

            return result;
        }

        private async Task<TryException<bool>> ValidarAprovacaoFinal
        (
            IEnumerable<CienciaUsuariosProvacao> aprovadorCiencia, SolicitacaoCienciaModel solicitacaoCienciaModel, 
            UsuarioModel usuarioModel, IDataTransaction transaction, CancellationToken cancellationToken
        )
        {   
            if (aprovadorCiencia.All(x => x.Aprovacao != null))
            {
                if (solicitacaoCienciaModel.IdTipoCiencia == (int)TipoCiencia.Prorrogação)
                {
                    var registraHistorico = await _solicitacaoSaidaMaterialRepository.RegistroHistoricoProrogacao(solicitacaoCienciaModel, cancellationToken);
                    if (registraHistorico.IsFailure) return registraHistorico.Failure;
                }

                if (aprovadorCiencia.Any(x => x.FlgRejeitado))
                    return await ProcessarRejeitado(solicitacaoCienciaModel, usuarioModel, transaction, cancellationToken);

                return await ProcessarAceito(solicitacaoCienciaModel, usuarioModel, transaction, cancellationToken);
            }

            return false;
        }

        private async Task<TryException<bool>> ProcessarRejeitado(SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, IDataTransaction transaction, CancellationToken cancellationToken)
        {
            solicitacaoCienciaModel.IdStatusCiencia = (int)Enums.StatusCiencia.Rejeitado;
            await _solicitacaoCienciaRepository.AtualizarCienciaStatus(solicitacaoCienciaModel, cancellationToken);

            switch (solicitacaoCienciaModel.IdTipoCiencia)
            {
                case (int)TipoCiencia.BaixaSemRetorno:
                case (int)TipoCiencia.Prorrogação:
                    await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.EmAberto, cancellationToken);
                    break;

                case (int)TipoCiencia.Cancelamento:
                    await ProcessarCancelamento(false, solicitacaoCienciaModel, usuarioModel, transaction, cancellationToken);
                    break;

                default:
                    break;
            }

            return false;
        }

        private async Task<TryException<bool>> ProcessarAceito(SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, IDataTransaction transaction, CancellationToken cancellationToken)
        {
            solicitacaoCienciaModel.IdStatusCiencia = (int)Enums.StatusCiencia.Concluido;
            await _solicitacaoCienciaRepository.AtualizarCienciaStatus(solicitacaoCienciaModel, cancellationToken);

            switch (solicitacaoCienciaModel.IdTipoCiencia)
            {
                case (int)TipoCiencia.BaixaSemRetorno:
                    await ProcessarBaixaSemRetorno(solicitacaoCienciaModel, usuarioModel.ActiveDirectoryId, cancellationToken);
                    return false;

                case (int)TipoCiencia.Cancelamento:
                    await ProcessarCancelamento(true, solicitacaoCienciaModel, usuarioModel, transaction, cancellationToken);
                    return true;
            }

            var atualizarStatus = await _solicitacaoSaidaMaterialRepository.AtualizarDataStatus(solicitacaoCienciaModel, SolicitacaoSaidaMaterialStatus.EmAberto, cancellationToken);
            if (atualizarStatus.IsFailure) return atualizarStatus.Failure;

            return false;
        }

        private async Task<TryException<Return>> ProcessarCancelamento(bool aceito, SolicitacaoCienciaModel solicitacaoCienciaModel, UsuarioModel usuarioModel, IDataTransaction transaction, CancellationToken cancellationToken)
        {
            transaction.Commit();

            SolicitacaoSaidaMaterialStatus solicitacaoSaidaMaterialStatus;
            var destinatario = _activeDirectoryExternalService.GetActiveDirectoryUsers(solicitacaoCienciaModel.NomeUsuario, new List<Guid> { solicitacaoCienciaModel.IdUsuario });

            var email = new EmailModel 
            { 
                Responsavel = solicitacaoCienciaModel.NomeUsuario,
                Nome = usuarioModel.Nome, 
                Numero = solicitacaoCienciaModel.NumeroMaterial.ToString(),
                DataEnvio = DateTime.Now.ToString("dd/MM/yyyy"),
                Destinatario = destinatario.Success.First()?.Email
            };

            if (aceito)
            {
                email.Template = _configuration.GetSection("TemplatesEmail:SolicitacaoSaidaMaterial-CancelamentoAceito").Value;
                email.Assunto = "GDocs - Cancelamento aceito";
                solicitacaoSaidaMaterialStatus = SolicitacaoSaidaMaterialStatus.EmConstrucao;  
            }
            else
            {
                var cienciasFI347 = await _solicitacaoCienciaRepository.ListarObsUsuarioPorCiencia(solicitacaoCienciaModel.Id, cancellationToken);

                if (cienciasFI347.IsFailure)
                    return cienciasFI347.Failure;

                var rejeicoesFI347 = cienciasFI347.Success.Where(a => a.FlgRejeitado).AsList();

                rejeicoesFI347.ForEach((rejeicao) =>
                {
                    var dadosUsuariosAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(null, new List<Guid> { rejeicao.UsuarioGuid });
                    var dadosUsuarios = dadosUsuariosAd.Success.AsList();

                    email.Texto.Add(new RejeicaoDiretorModel
                    {
                        NomeDiretor = dadosUsuarios[0].Nome,
                        Observacao = rejeicao.Observacao,
                        DataRejeicao = $"{rejeicao.DataAprovacao: dd/MM/yyyy}"
                    });
                });

                email.Template = _configuration.GetSection("TemplatesEmail:SolicitacaoSaidaMaterial-CancelamentoRejeitado").Value;
                email.Assunto = "GDocs - Cancelamento rejeitado";
                solicitacaoSaidaMaterialStatus = SolicitacaoSaidaMaterialStatus.PendenteSaida;
            }

            await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, solicitacaoSaidaMaterialStatus, cancellationToken);
            await _emailExternalService.Enviar(email);

            return Return.Empty;
        }

        private async Task<TryException<Return>> ProcessarBaixaSemRetorno(SolicitacaoCienciaModel solicitacaoCienciaModel, Guid usuarioLogadoId, CancellationToken cancellationToken)
        {
            var acaoModel = SaidaMaterialacaoManual(solicitacaoCienciaModel, SaidaMaterialTipoAcao.BaixaMaterialSemRetorno);

            var registraAcao = await _solicitacaoSaidaMaterialAcaoService.InserirAcao(usuarioLogadoId, acaoModel, cancellationToken);
            if (registraAcao.IsFailure) return registraAcao.Failure;

            var ItensDaSolicitacaoMaterial = await _solicitacaoSaidaMaterialItemRepository.ObterPorIdSolicitacaoSaidaMaterial(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, cancellationToken);
            if (ItensDaSolicitacaoMaterial.IsFailure) return ItensDaSolicitacaoMaterial.Failure;

            var QuantidadeItemMaterialBaixado = await _solicitacaoSaidaMaterialItemRepository.ObterQuantidadeItemComBaixa(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, cancellationToken);
            if (QuantidadeItemMaterialBaixado.IsFailure) return QuantidadeItemMaterialBaixado.Failure;

            if (ItensDaSolicitacaoMaterial.Success.Count() == QuantidadeItemMaterialBaixado.Success)
            {
                await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.Concluido, cancellationToken);
                return Return.Empty;
            }

            await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.EmAberto, cancellationToken, true);

            return Return.Empty;
        }
        private SolicitacaoSaidaMaterialAcaoModel SaidaMaterialacaoManual(SolicitacaoCienciaModel solicitacaoCienciaModel, SaidaMaterialTipoAcao status)
        {
            var acao = new SolicitacaoSaidaMaterialAcaoModel();
            acao.IdSaidaMaterialTipoAcao = status.ToInt32();
            acao.IdSolicitacaoSaidaMaterial = solicitacaoCienciaModel.IdSolicitacaoSaidaMaterial;

            foreach (var item in solicitacaoCienciaModel.Itens)
                acao.AcaoItems.Add(new SolicitacaoSaidaMaterialAcaoItemModel(item.IdSolicitacaoSaidaMaterialItem));

            return acao;
        }

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsPorUsuario(int IdSolicitacaoCiencia, CancellationToken cancellationToken) => await
            _solicitacaoCienciaRepository.ListarObsUsuarioPorCiencia(IdSolicitacaoCiencia, cancellationToken);

        public async Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int solicitacaoSaidaMaterialId, CancellationToken cancellationToken)
         => await _solicitacaoCienciaRepository.ObterAprovadoresCienciaCancelamento(solicitacaoSaidaMaterialId, cancellationToken);
    }
}
