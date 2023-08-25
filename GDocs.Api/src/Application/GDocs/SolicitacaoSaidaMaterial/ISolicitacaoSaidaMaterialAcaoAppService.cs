﻿using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialAcaoAppService : IApplicationService
    {
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoTipoModel>>> ListarAcaoTipo();
        Task<TryException<int>> InserirAcao(Guid usuarioLogado, SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao tipoAcao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<HistoricoResponseModel>> ObterHistoricoMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);

    }
}
