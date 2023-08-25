using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class ProcessoAssinaturaDocumentoOrigemRepository : Repository, IProcessoAssinaturaDocumentoOrigemRepository
    {
        public ProcessoAssinaturaDocumentoOrigemRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork) { }

        public async Task<TryException<ProcessoAssinaturaDocumentoOrigemModel>> ListarPorProcessoAssinaturaDocumentoOrigemIdENome(string ProcessoAssinaturaDocumentoOrigemId, string ProcessoAssinaturaDocumentoOrigemNome, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoOrigemModel>(
                new CommandDefinition(
                    commandText: $@"SELECT [pad_idt] AS ProcessoAssinaturaDocumentoId
                                          ,[pado_origem_idt] AS ProcessoAssinaturaDocumentoOrigemId
                                          ,[pado_origem_nome] AS ProcessoAssinaturaDocumentoOrigemNome
                                          ,[pado_flg_ativo] AS Ativo
                                          ,[pado_dat_criacao] AS DataCriacao
                                          ,[pado_dat_atualizacao] AS DataAtualizacao
                                    FROM 
                                        [dbo].[tb_pado_processo_assinatura_documento_origem]
                                    WHERE
                                        [pado_flg_ativo] = 1
                                        AND [pado_origem_idt] =  @ProcessoAssinaturaDocumentoOrigemId 
                                        AND [pado_origem_nome] = @ProcessoAssinaturaDocumentoOrigemNome",
                    parameters: new
                    {
                        ProcessoAssinaturaDocumentoOrigemId,
                        ProcessoAssinaturaDocumentoOrigemNome
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return result?.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoOrigemModel>>> ListarProcessosAssinaturaPorOrigem(string identificadorOrigem, string nomeOrigem, CancellationToken cancellationToken)
            => (await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoOrigemModel>(
                new CommandDefinition(
                    commandText: @"
                        SELECT 
	                        pado.[pad_idt] ProcessoAssinaturaDocumentoId,
	                        pado.[pado_origem_idt] ProcessoAssinaturaDocumentoOrigemId,
	                        pado.[pado_origem_nome] ProcessoAssinaturaDocumentoOrigemNome
                        FROM 
	                        [dbo].[tb_pado_processo_assinatura_documento_origem] pado
                        WHERE
	                        pado.[pado_origem_idt] = @identificadorOrigem
	                        AND pado.[pado_origem_nome] = @nomeOrigem
	                        AND pado.[pado_flg_ativo] = 1",
                    parameters: new
                    {
                        identificadorOrigem,
                        nomeOrigem
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                ))).ToCollection();

        public async Task<TryException<Return>> Inserir(ProcessoAssinaturaDocumentoOrigemModel processoAssinaturaDocumentoOrigemModel, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"INSERT [dbo].[tb_pado_processo_assinatura_documento_origem] (pad_idt,pado_origem_idt,pado_origem_nome) VALUES(@ProcessoAssinaturaDocumentoId, @ProcessoAssinaturaDocumentoOrigemId, @ProcessoAssinaturaDocumentoOrigemNome);",
                    parameters: new
                    {
                        processoAssinaturaDocumentoOrigemModel.ProcessoAssinaturaDocumentoId,
                        processoAssinaturaDocumentoOrigemModel.ProcessoAssinaturaDocumentoOrigemId,
                        processoAssinaturaDocumentoOrigemModel.ProcessoAssinaturaDocumentoOrigemNome
                    },
                    transaction: Transaction,
                    cancellationToken: cancellationToken
                )
            );

            return Return.Empty;
        }

        public async Task<TryException<Return>> InativarAssDocumentoOrigem(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            await _db.Connection.ExecuteAsync(
               new CommandDefinition(
                   commandText: @"
                                UPDATE 
                                    tb_pado_processo_assinatura_documento_origem 
                                SET  
                                    pado_flg_ativo = 0 
                                WHERE
                                    pad_idt = @Id
                                    AND pado_flg_ativo = 1",
                   transaction: Transaction,
                   cancellationToken: cancellationToken,
                   parameters: new
                   {
                       Id = processoAssinaturaDocumentoId,
                   }
               ));

            return Return.Empty;
        }
    }
}
