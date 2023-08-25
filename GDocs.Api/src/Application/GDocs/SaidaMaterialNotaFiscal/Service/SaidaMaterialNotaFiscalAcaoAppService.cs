using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Service
{
    public class SaidaMaterialNotaFiscalAcaoAppService : ISaidaMaterialNotaFiscalAcaoAppService
    {
        private readonly ISaidaMaterialNotaFiscalAcaoService _saidaMaterialNotaFiscalAcaoService;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;

        public SaidaMaterialNotaFiscalAcaoAppService(ISaidaMaterialNotaFiscalAcaoService saidaMaterialNotaFiscalAcaoService, 
            IActiveDirectoryExternalService activeDirectoryExternalService)
        {
            _saidaMaterialNotaFiscalAcaoService = saidaMaterialNotaFiscalAcaoService;
            _activeDirectoryExternalService = activeDirectoryExternalService;
        }

        public async Task<TryException<int>> InserirAcao(Guid usuarioLogado, SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken)
            => await _saidaMaterialNotaFiscalAcaoService.InserirAcao(usuarioLogado, saidaMaterialNotaFiscalAcaoModel, cancellationToken);

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalTipoAcaoModel>>> ListarAcaoTipo()
            => await _saidaMaterialNotaFiscalAcaoService.ListarAcaoTipo();

        public async Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao tipoAcao, CancellationToken cancellationToken)
            => await _saidaMaterialNotaFiscalAcaoService.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(idSaidaMaterialNotaFiscal, tipoAcao, cancellationToken);

        public async Task<TryException<IEnumerable<HistoricoTrocaAnexoSaidaModel>>> ListarHistoricoAnexoSaida(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {

           var listaHistoricoAnexo =  await _saidaMaterialNotaFiscalAcaoService.ListarHistoricoAnexoSaida(idSolicitacaoSaidaMaterial, cancellationToken);

            if (listaHistoricoAnexo.IsFailure)
                return listaHistoricoAnexo.Failure;

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, listaHistoricoAnexo.Success.Select(x => x.GuidAutor).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var results = listaHistoricoAnexo.Success.Join(usuarios.Success,
                      d => d.GuidAutor,
                      u => u.Guid,
                      (mat, usu) => mat.DefinirNomeAutor(usu.Nome)
                    );


            return results?.ToCollection();

        }


        public async Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
            => await _saidaMaterialNotaFiscalAcaoService.ListarHistoricoBaixaSemRetorno(idSolicitacaoSaidaMaterial, cancellationToken);

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
            => await _saidaMaterialNotaFiscalAcaoService.LitarAcaoSaidaEhRetorno(idSaidaMaterialNotaFiscal, cancellationToken);
    }
}
