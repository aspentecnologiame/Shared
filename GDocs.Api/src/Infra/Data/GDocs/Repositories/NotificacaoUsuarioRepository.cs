using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories
{
    internal class NotificacaoUsuarioRepository : Repository, INotificacaoUsuarioRepository
    {
        public NotificacaoUsuarioRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<NotificacaoRelatorioModel>>> ObterRelatoriosNaoLidosPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryAsync<NotificacaoRelatorioModel>(
                new CommandDefinition(
                    commandText: $@"
                        SELECT 
                            [ntu_idt] AS IdNotificacaoUsuario
                            ,[ntu].[rel_idt] as IdRelatorio
                            ,[ntu_id_usuario] AS IdUsuario
                            ,[ntu_lido] AS Lido
                            ,[ntu_ativo] AS Ativo
                            ,[ntu_dat_leitura] AS DataLeitura
                            ,[ntu_dat_criacao] AS DataCriacao
                            ,[ntu_dat_atualizacao] AS DataAtualizacao
                            ,[rel_nome] AS Nome
                            ,[rel_url] AS Url
                            ,[rel_parametros] AS Parametros
                        FROM 
                            [dbo].[tb_ntu_notificacao_usuario] ntu
                        INNER JOIN 
                            [dbo].[tb_rel_relatorio] rel
                        ON
                            [rel].[rel_idt] = [ntu].[rel_idt] 
                        WHERE
                            [ntu_id_usuario] = @IdUsuario AND [ntu_lido] = 0
                        AND 
                            [ntu_ativo] = 1",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        IdUsuario = idUsuario
                    }
                )
            );

            return result?.ToCollection();
        }

        public async Task<TryException<NotificacaoRelatorioModel>> ObterRelatorioNaoLidoPorIdNotificacao(int idNotificacaoUsuario, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryFirstAsync<NotificacaoRelatorioModel>(
                new CommandDefinition(
                    commandText: $@"
                        SELECT 
                            [ntu_idt] AS IdNotificacaoUsuario
                            ,[ntu].[rel_idt] as IdRelatorio
                            ,[ntu_id_usuario] AS IdUsuario
                            ,[ntu_lido] AS Lido
                            ,[ntu_ativo] AS Ativo
                            ,[ntu_dat_leitura] AS DataLeitura
                            ,[ntu_dat_criacao] AS DataCriacao
                            ,[ntu_dat_atualizacao] AS DataAtualizacao
                            ,[rel_nome] AS Nome
                            ,[rel_url] AS Url
                            ,[rel_parametros] AS Parametros
                      FROM 
                            [dbo].[tb_ntu_notificacao_usuario] ntu
                      INNER JOIN 
                            [dbo].[tb_rel_relatorio] rel
                      ON
                            [rel].[rel_idt] = [ntu].[rel_idt]
                      WHERE 
                            [ntu_idt] = @IdNotificacaoUsuario AND [ntu_lido] = 0
                      AND 
                            [ntu_ativo] = 1",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        IdNotificacaoUsuario = idNotificacaoUsuario
                    }
                )
            );

            return result;
        }

        public async Task<TryException<int>> ObterQuantidadeRelatoriosNaoLidosPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken)
        {
            var result = await _db.Connection.QueryFirstAsync<int>(
                new CommandDefinition(
                    commandText: @"
                            SELECT 
                                CAST(COUNT([ntu_idt]) AS INT) AS ids
                            FROM 
                                [dbo].[tb_ntu_notificacao_usuario] ntu
                            WHERE 
                                [ntu].[ntu_id_usuario] = @IdUsuario 
                            AND 
                                [ntu_lido] = 0
                            AND 
                                [ntu_ativo] = 1",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        IdUsuario = idUsuario
                    }
                )
            );

            return result;
        }

        public async Task<TryException<int>> AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(int idNotificacaoUsuario, CancellationToken cancellationToken)
        {
            return await _db.Connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: @"
                            UPDATE [dbo].[tb_ntu_notificacao_usuario] SET [ntu_lido] = 1, [ntu_dat_atualizacao] = GETDATE(), [ntu_dat_leitura] = GETDATE() WHERE [ntu_idt] = @IdNotificacaoUsuario",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        IdNotificacaoUsuario = idNotificacaoUsuario
                    }
                )
            );
        }

        public async Task<TryException<RelatorioModel>> ObterParametrosDoRelatorio(CancellationToken cancellationToken, RelatorioNotificacao relatorioNotificacao)
        {
            return await _db.Connection.QueryFirstAsync<RelatorioModel>(
                new CommandDefinition(
                    commandText: $@"
                                    SELECT 
	                                    [rel].[rel_url] as Url,
	                                    [rel].[rel_parametros] as Parametros
                                    FROM
	                                    [tb_rel_relatorio] rel
                                    WHERE
	                                    [rel].[rel_idt] = @RelatorioNotificacao",
                    transaction: Transaction,
                    cancellationToken: cancellationToken,
                    parameters: new
                    {
                        RelatorioNotificacao = relatorioNotificacao
                    }
                )
            );
        }
    }
}
