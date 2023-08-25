using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.Services;
using System.Linq;
using ICE.GDocs.Common.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Service
{
    public class SaidaMateriaNotaFiscalAppService : ISaidaMaterialNotaFiscalAppService
    {
        private readonly ISaidaMateriaNotaFiscalService _saidaMateriaNotaFiscalService;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;
        private readonly ISaidaMaterialNotaFiscalAcaoAppService _saidaMaterialNotaFiscalAcaoAppService;


        private const string CANCELAR_CODERRO = "SaidaMaterialNF:acao:cancelar"; 
        private readonly ILogService _logService;


        public SaidaMateriaNotaFiscalAppService(ISaidaMateriaNotaFiscalService saidaMateriaNotaFiscalService, IActiveDirectoryExternalService activeDirectoryExternalService, IUsuarioService usuarioService, ILogService logService, ISaidaMaterialNotaFiscalAcaoAppService saidaMaterialNotaFiscalAcaoAppService)
        {
            _saidaMateriaNotaFiscalService = saidaMateriaNotaFiscalService;
            _activeDirectoryExternalService = activeDirectoryExternalService;
            _saidaMaterialNotaFiscalAcaoAppService = saidaMaterialNotaFiscalAcaoAppService;
            _logService = logService;
        }


        public async Task<TryException<Return>> EfetivarCancelamento(SaidaMaterialNotaFiscalModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {

            if (solicitacaoSaidaMaterialModel.StatusId != SaidaMaterialNotaFiscalStatus.PendenteNFCancelamento.ToInt32())
                throw new BusinessException($"Não é possivel efetivar o cancelamento do material que não se encontra no status: {SaidaMaterialNotaFiscalStatus.PendenteNFSaida.GetDescription()}");


            var efetivarCancelamento = await _saidaMateriaNotaFiscalService.EfetivarCancelamento(solicitacaoSaidaMaterialModel, cancellationToken);

            if (efetivarCancelamento.IsFailure)
                return efetivarCancelamento.Failure;


            return Return.Empty;
        }


        public async Task<TryException<Return>> CancelarOuSolicitarCancelamento(int saidaMaterialNfId, string motivo, CancellationToken cancellationToken)
        {
            var obterSaidaMaterialPorId = await ObterMaterialNotaFiscalPorId(saidaMaterialNfId, cancellationToken);

            if (obterSaidaMaterialPorId.IsFailure)
                return obterSaidaMaterialPorId.Failure;

            if (obterSaidaMaterialPorId.Success == null)
                return new BusinessException(CANCELAR_CODERRO, "Solicitação de saída de material com nota fiscal não encontrada.");

            if (obterSaidaMaterialPorId.Success.StatusId != SaidaMaterialNotaFiscalStatus.PendenteNFSaida.ToInt32() && 
                obterSaidaMaterialPorId.Success.StatusId != SaidaMaterialNotaFiscalStatus.PendenteSaida.ToInt32())
                return new BusinessException(CANCELAR_CODERRO, "Solicitação de saída de material com nota fiscal se encontra em um status que não se pode cancelar.");

            var saidaMaterialNf = obterSaidaMaterialPorId.Success;

            var CancelarSolicitacao = await _saidaMateriaNotaFiscalService.CancelarOuSolicitarCancelamento(saidaMaterialNf, motivo, cancellationToken);

            if (CancelarSolicitacao.IsFailure)
                return CancelarSolicitacao.Failure;


            return Return.Empty;

        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> ConsultaMaterialNotaFiscalPorFiltro(
            SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFiltroModel, CancellationToken cancellationToken)
        {
            TryException<IEnumerable<SaidaMaterialNotaFiscalModel>> response;
            saidaMaterialNotaFiscalFiltroModel.ExibirNaOrdem = await this.SetarOrdenacaoRelatorioSaidaMaterial(saidaMaterialNotaFiscalFiltroModel.ExibirNaOrdem);
            response = await _saidaMateriaNotaFiscalService.ConsultaMaterialNotaFiscalPorFiltro(saidaMaterialNotaFiscalFiltroModel, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            if(response.Success?.Count() == 0 && saidaMaterialNotaFiscalFiltroModel.Numero > 0)
                response = await _saidaMateriaNotaFiscalService.ConsultaMaterialNotaFiscalPorFiltro(saidaMaterialNotaFiscalFiltroModel, cancellationToken, true);

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, response.Success.Select(x => x.GuidAutor).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var results = response.Success.Join(usuarios.Success,
                      d => d.GuidAutor,
            u => u.Guid,
            (mat, usu) => mat.DefinirNomeAutor(usu.Nome)
            );


            return results?.ToCollection();

        }
       

        public async Task<TryException<SaidaMaterialNotaFiscalModel>> Inserir(SaidaMaterialNotaFiscalModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            var result = await _saidaMateriaNotaFiscalService.Inserir(solicitacaoSaidaMaterialModel, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            return result.Success;
        }

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarModalidadeFrete(CancellationToken cancellationToken) =>
            await _saidaMateriaNotaFiscalService.ListarModalidadeFrete(cancellationToken);
         

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarNatureza(CancellationToken cancellationToken) => 
            await _saidaMateriaNotaFiscalService.ListarNatureza(cancellationToken);

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterial(CancellationToken cancellationToken) =>
            await _saidaMateriaNotaFiscalService.ListarStatusMaterial(cancellationToken);

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterialFiltro(UsuarioModel usuario, CancellationToken cancellationToken) =>
            await _saidaMateriaNotaFiscalService.ListarStatusMaterialFiltro(usuario, cancellationToken);

        public async Task<TryException<IEnumerable<SaidaMaterialArquivoModel>>> ObterBase64DoPdf(int saidaMaterialNfId, CancellationToken cancellationToken) =>
            await _saidaMateriaNotaFiscalService.ObterBase64DoPdf(saidaMaterialNfId, cancellationToken);
    

        public async Task<TryException<SaidaMaterialNotaFiscalModel>> ObterMaterialNotaFiscalPorId(int id, CancellationToken cancellationToken) 
        {

            var result  = await _saidaMateriaNotaFiscalService.ObterMaterialNotaFiscalPorId(id, cancellationToken);
            
            if (result.IsFailure)
                return result.Failure;

            return result;
        } 

        public async Task<TryException<Return>> SalvarArquivosUpload(IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken)
        {
            var saidaMatarialNf = await _saidaMateriaNotaFiscalService.ObterMaterialNotaFiscalPorId(saidaMaterialArquivoModel.First().SaidaMaterialNfId, cancellationToken);

            if (saidaMatarialNf.IsFailure)
                return saidaMatarialNf.Failure;

            if (saidaMatarialNf.Success.StatusId == SaidaMaterialNotaFiscalStatus.PendenteNFRetorno.ToInt32())
            {
                var resultRetorno  = await _saidaMateriaNotaFiscalService.SalvarArquivosUploadRetorno(saidaMatarialNf.Success, saidaMaterialArquivoModel, uploadBasePath, usuarioLogado, cancellationToken);
                if (resultRetorno.IsFailure)
                    return resultRetorno.Failure;

                return resultRetorno.Success;
            }

            var resultAnexoSaida =   await _saidaMateriaNotaFiscalService.SalvarArquivosUploadSaida(saidaMaterialArquivoModel, uploadBasePath, usuarioLogado, cancellationToken);
            
           if(resultAnexoSaida.IsFailure) 
             return resultAnexoSaida.Failure;  

           return resultAnexoSaida.Success;
        }

   

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> PesquisarSaidaMateriaisNotaFiscalPorFiltro(
                  SaidaMaterialNotaFiscalFilterModel filtro,
                  CancellationToken cancellationToken
              )
        {
            await _logService.AdicionarRastreabilidade(filtro, "Iniciando a busca de solicitações de saída de material com o filtro Selecionado.");

            filtro.ExibirNaOrdem = await this.SetarOrdenacaoRelatorioSaidaMaterial(filtro.ExibirNaOrdem);
            var result = await _saidaMateriaNotaFiscalService.ConsultaMaterialNotaFiscalPorFiltro(filtro, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            await _logService.AdicionarRastreabilidade(filtro, $"{result.Success.Count()} resultado encontrado.");

            var users = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, result.Success.Select(x => x.GuidAutor).Distinct().AsList());

            if (users.IsFailure)
                return users.Failure;

            var results = result.Success.Join(users.Success,
                      d => d.GuidAutor,
                      u => u.Guid,
                      (mat, usu) => mat.DefinirNomeResponsavel(usu.Nome)
                    );

            await _logService.AdicionarRastreabilidade(filtro, $"Total de {results.Count()} solicitações de saída de material após junção com os autores ad.");

            return results?.ToCollection();
        }

        private async Task<SaidaMaterialNotaFiscalOrdenacaoModel> SetarOrdenacaoRelatorioSaidaMaterial(SaidaMaterialNotaFiscalOrdenacaoModel exibirNaOrdem)
        {
            exibirNaOrdem ??= new SaidaMaterialNotaFiscalOrdenacaoModel();
            switch (exibirNaOrdem.Active)
            {
                case "retornoNf": exibirNaOrdem.OrderBy = $"smnf.smnf_dat_retorno {exibirNaOrdem.Direction}, smnf.smnf_num asc"; break;

                case "numeroNf": exibirNaOrdem.OrderBy = $"smnf.smnf_num {exibirNaOrdem.Direction}"; break;

                case "tipoSaidaNf": exibirNaOrdem.OrderBy = $"tiposaida {exibirNaOrdem.Direction}, smnf.smnf_num asc"; break;

                case "destinoNf": exibirNaOrdem.OrderBy = $"smnf.smnf_destino {exibirNaOrdem.Direction}, smnf.smnf_num asc"; break;

                case "statusNf": exibirNaOrdem.OrderBy = $"stsmnf.stsmnf_des {exibirNaOrdem.Direction}, stsmnf.stsmnf_num asc"; break;

                case "dataAcaoNf": exibirNaOrdem.OrderBy = $"smnfa.smnfa_dat_acao {exibirNaOrdem.Direction}, smnf.smnf_num asc"; break;

                default:
                    exibirNaOrdem.Direction = "Desc";
                    exibirNaOrdem.OrderBy = $"smnf.smnf_dat_criacao {exibirNaOrdem.Direction}, smnf.smnf_num asc";
                    break;
            }

            return await Task.FromResult(exibirNaOrdem);
        }

        public async Task<TryException<SaidaMaterialNotaFiscalHistoricoResponseModel>> ObterHistoricoMaterialNf(int saidaMaterialNfId, CancellationToken cancellationToken)
        {
            var saidaMaterialNotaFiscal = await ObterMaterialNotaFiscalPorId(saidaMaterialNfId, cancellationToken);
            if (saidaMaterialNotaFiscal.IsFailure)
                return saidaMaterialNotaFiscal.Failure;

            var listaMaterialNotaFiscalAcao = await _saidaMaterialNotaFiscalAcaoAppService.LitarAcaoSaidaEhRetorno(saidaMaterialNfId, cancellationToken);
            if (listaMaterialNotaFiscalAcao.IsFailure)
                return listaMaterialNotaFiscalAcao.Failure;


            var historicoProrrogacaoNotaFiscal = await _saidaMaterialNotaFiscalAcaoAppService.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(saidaMaterialNfId,
                                                                                                                                                      SaidaMaterialNotaFiscalTipoAcao.SolicitacaoProrrogacao, cancellationToken);

            if (historicoProrrogacaoNotaFiscal.IsFailure)
                return historicoProrrogacaoNotaFiscal.Failure;

            var historicoBaixaSemRetornoNotaFiscal = await _saidaMaterialNotaFiscalAcaoAppService.ListarHistoricoBaixaSemRetorno(saidaMaterialNfId, cancellationToken);

            if (historicoBaixaSemRetornoNotaFiscal.IsFailure)
                return historicoBaixaSemRetornoNotaFiscal.Failure;

            var historicoAnexoSaida = await _saidaMaterialNotaFiscalAcaoAppService.ListarHistoricoAnexoSaida(saidaMaterialNfId, cancellationToken);
            if (historicoAnexoSaida.IsFailure)
                return historicoAnexoSaida.Failure;


            ProcessarStatus(saidaMaterialNotaFiscal, listaMaterialNotaFiscalAcao, historicoBaixaSemRetornoNotaFiscal);

            return new SaidaMaterialNotaFiscalHistoricoResponseModel()
            {
                SaidaMaterialNotaFiscal = saidaMaterialNotaFiscal.Success,
                SaidaMaterialNotaFiscalAcoes = listaMaterialNotaFiscalAcao.Success,
                HistoricoProrrogacoes = historicoProrrogacaoNotaFiscal.Success,
                HistoricoBaixaSemRetorno = historicoBaixaSemRetornoNotaFiscal.Success,
                HistoricoTrocaAnexoSaida = historicoAnexoSaida.Success
            };

        }

        private void ProcessarStatus(TryException<SaidaMaterialNotaFiscalModel> saidaMaterialNotaFiscal, TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoModel>> listaMaterialNotaFiscalAcao, TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>> historicoBaixaSemRetornoNotaFiscal)
        {
            saidaMaterialNotaFiscal.Success.ItemMaterialNf.ForEach(item =>
            {

                if(listaMaterialNotaFiscalAcao.Success.Any(x => x.IdSaidaMaterialNotaFiscalAcaoItem == item.Id && x.IdSaidaMaterialNotaFiscalTipoAcao == (int)SaidaMaterialNotaFiscalTipoAcao.RegistroSaida))
                    item.Status = StatusHistoricoMaterial.PendenteRetorno.GetDescription();

                if(listaMaterialNotaFiscalAcao.Success.Any(x => x.IdSaidaMaterialNotaFiscalAcaoItem == item.Id && x.IdSaidaMaterialNotaFiscalTipoAcao == (int)SaidaMaterialNotaFiscalTipoAcao.RegistroRetorno))
                    item.Status = StatusHistoricoMaterial.Retornado.GetDescription();

                if(historicoBaixaSemRetornoNotaFiscal.Success.Any(x => x.SaidaMaterialNotaFiscalItemId == item.Id && x.StatusId == (int)StatusCiencia.Concluido))
                    item.Status = StatusHistoricoMaterial.BaixaSemRetorno.GetDescription();

                if(saidaMaterialNotaFiscal.Success.StatusId == (int)SaidaMaterialNotaFiscalStatus.Concluida && item.Status != StatusHistoricoMaterial.BaixaSemRetorno.GetDescription())
                    item.Status = StatusHistoricoMaterial.Concluido.GetDescription();

                if(String.IsNullOrEmpty(item.Status))
                    item.Status = StatusHistoricoMaterial.PendenteSaida.GetDescription();

            });
        }
    }
}
