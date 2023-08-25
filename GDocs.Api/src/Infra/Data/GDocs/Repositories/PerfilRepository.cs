using Dapper;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories
{
    internal class PerfilRepository : Repository, IPerfilRepository
    {
        private const int ID_PERFIL_DEFAULT = 1;
        public PerfilRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<PerfilModel>>> ListarTodosPerfisAtivos(CancellationToken cancellationToken)
        {
            var perfis = await _db.Connection.QueryAsync<PerfilModel>(
                new CommandDefinition(
                    commandText: @"SELECT 
						    per_idt [Id]
						    ,per_des [Descricao]
                            ,per_peso [Peso]
						    ,per_flg_ativo [Ativo]
						    ,per_dat_inclusao [DataInclusao]
						    ,per_dat_alteracao [DataAlteracao]
					    FROM [dbo].[tb_per_perfil]
                        WHERE per_flg_ativo = 1 AND per_idt <> 1
                        ORDER BY per_des",
                    cancellationToken: cancellationToken
                )
            );

            return perfis.ToCollection();
        }

        public async Task<TryException<UsuarioModel>> ListarPerfisUsuarioPorGuid(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var guids = new List<Guid> { activeDirectoryId };

            var usuariosPerfis = await ListarPerfisUsuariosPorGuids(guids, cancellationToken);

            if (usuariosPerfis.IsFailure)
                return usuariosPerfis.Failure;

            return usuariosPerfis.Success.FirstOrDefault();
        }

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuariosPorGuids(IEnumerable<Guid> activeDirectoryIds, CancellationToken cancellationToken)
            => await ListarPerfisUsuarios(
                new CommandDefinition(
                    commandText: $@"SELECT 						
                        [USP].[usp_guid_ad] as [ActiveDirectoryId],
                        [USP].[usp_nome_ad] as [Nome],
                        [USP].[usp_flg_ativo] as [Status],
                        [USP].[usp_dat_inclusao] as [DataInclusao],
                        [USP].[usp_dat_alteracao] as [DataAtualizacao],

                        [PER].[per_idt] as [Id],
                        [PER].[per_des] as [Descricao],
                        [PER].[per_peso] as [Peso],
                        [PER].[per_flg_ativo] as [Ativo]
                    FROM 
                        [dbo].[tb_usp_usuario_perfil] [USP]
                    JOIN [dbo].[tb_per_perfil] [PER] ON [PER].[per_idt] = [USP].[per_idt] AND [PER].[per_flg_ativo] = 1
                    WHERE [USP].[usp_flg_ativo] = 1 AND [USP].[usp_guid_ad] IN @activeDirectoryIds
                    ORDER BY [USP].[usp_guid_ad] , [PER].[per_idt]"
                    ,
                    parameters: new { activeDirectoryIds },
                    cancellationToken: cancellationToken
                ));

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuariosPorPerfil(int perfilId, CancellationToken cancellationToken)
            => await ListarPerfisUsuarios(
                new CommandDefinition(
                    commandText: $@";WITH cte_guid_perfil_ativo([usp_guid_ad],[per_idt])
					    AS(
						    SELECT 						
							    [USP].[usp_guid_ad],
							    [USP].[per_idt]
						    FROM 
							    [dbo].[tb_usp_usuario_perfil] [USP]
						    WHERE [USP].[usp_flg_ativo] = 1
					    )
					    SELECT DISTINCT
						    [USP].[usp_guid_ad] as [ActiveDirectoryId],
						    [USP].[usp_flg_ativo] as [Status],
						    [USP].[usp_dat_inclusao] as [DataInclusao],
						    [USP].[usp_dat_alteracao] as [DataAtualizacao],
		
						    [PER].[per_idt] as [Id],
						    [PER].[per_des] as [Descricao],
                            [PER].[per_peso] as [Peso],
						    [PER].[per_flg_ativo] as [Ativo]
					    FROM cte_guid_perfil_ativo [CTE]
					    INNER JOIN [dbo].[tb_usp_usuario_perfil] [USP] ON [USP].[usp_guid_ad] = [CTE].[usp_guid_ad]
					    JOIN [dbo].[tb_per_perfil] [PER] ON [PER].[per_idt] = [USP].[per_idt] AND [PER].[per_flg_ativo] = 1
					    WHERE [USP].[usp_flg_ativo] = 1 AND ([CTE].per_idt = @perfilId OR @perfilId = 0)"
                    ,
                    parameters: new { perfilId },
                    cancellationToken: cancellationToken
                ));

        private async Task<TryException<IEnumerable<UsuarioModel>>> ListarPerfisUsuarios(CommandDefinition commandDefinition)
            => (await _db.Connection.QueryAsync<UsuarioModel, PerfilModel, UsuarioModel>(
                        commandDefinition,
                        (usuarioModel, perfilModel) =>
                        {
                            usuarioModel.Perfis = new List<PerfilModel>() { perfilModel };
                            return usuarioModel;
                        }
                    )
                ).GroupBy(grp => grp.ActiveDirectoryId)
                .Select(usuarioGroup =>
                {
                    var usuario = usuarioGroup.First();
                    usuario.Perfis = usuarioGroup.SelectMany(usu => usu.Perfis).ToList();
                    return usuario;

                })
                .ToCollection();

        public async Task<TryException<UsuarioModel>> SalvarPerfisUsuario(UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            try
            {
                var registros = new List<string>
                {
                    $"('{usuarioModel.ActiveDirectoryId}', {ID_PERFIL_DEFAULT}, 1, GETDATE(), GETDATE(), '{usuarioModel.Nome}','{usuarioModel.Email}')"
                };

                registros.AddRange(
                    usuarioModel.Perfis
                        .Where(per => per.Id != ID_PERFIL_DEFAULT)
                        .ConvertAll(per => $"('{usuarioModel.ActiveDirectoryId}', {per.Id}, {(per.Ativo ? 1 : 0)}, GETDATE(), GETDATE(),'{usuarioModel.Nome}','{usuarioModel.Email}')")
                );

                var sql = $@"	
							MERGE INTO [dbo].[tb_usp_usuario_perfil] AS Target
							USING 
							(
							VALUES
								{string.Join(",", registros)}
							) AS Source 
							(usp_guid_ad, per_idt, usp_flg_ativo, usp_dat_inclusao, usp_dat_alteracao, usp_nome_ad,usp_email_ad) ON Target.[usp_guid_ad] = Source.[usp_guid_ad] and Target.[per_idt] = Source.[per_idt]

							--altera registros que existem no Target e no Source
							WHEN MATCHED THEN
								UPDATE SET usp_flg_ativo = Source.usp_flg_ativo , usp_dat_alteracao = GETDATE()

							--inseri novos registros que não existem no target e existem no source
							WHEN NOT MATCHED BY TARGET THEN
								INSERT ([usp_guid_ad],[per_idt],[usp_nome_ad],[usp_email_ad]) VALUES ([usp_guid_ad], [per_idt],[usp_nome_ad],[usp_email_ad])

							--inativa registros que somente existem no target e não existem no source
							WHEN NOT MATCHED BY SOURCE AND [usp_guid_ad] = @usuarioAdId THEN
								UPDATE SET usp_flg_ativo = 0 , usp_dat_alteracao = GETDATE();
							
					";

                await _db.Connection.ExecuteScalarAsync(new CommandDefinition(
                    commandText: sql,
                    parameters: new
                    {
                        usuarioAdId = usuarioModel.ActiveDirectoryId,
                        perfisParaAtualizar = usuarioModel.Perfis.Count()
                    },
                    cancellationToken: cancellationToken)
                );

                return usuarioModel;
            }
            catch (Exception ex)
            {
                return new BusinessException(
                    "crud-usuarios-erro",
                    $"Erro: IUsuarioRepository.CrudUsuario",
                    ex);
            }
        }

        public async Task<TryException<IEnumerable<FuncionalidadeModel>>> ListarFuncionalidadesPorPerfis(IEnumerable<int> perfis, CancellationToken cancellationToken)
        {
            return (await _db.Connection.QueryAsync<FuncionalidadeModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT DISTINCT
						        fun.func_idt [Id]
						        ,fun.func_chave [Chave]
						        ,fun.func_texto [Texto]
						        ,fun.func_icone [Icone]
						        ,fun.func_rota [Rota]
						        ,fun.tfu_idt [TipoFuncionalidadeId]
						        ,fun.func_idt_pai [IdPai]
						        ,fun.func_ordem [Ordem]
					        FROM [dbo].[tb_func_funcionalidade] fun
					        INNER JOIN [dbo].[tb_tfu_tipo_funcionalidade] tfu ON tfu.tfu_idt = fun.tfu_idt
					        INNER JOIN [dbo].[tb_pfu_perfil_funcionalidade] pfu ON pfu.func_idt = fun.func_idt
					        WHERE fun.func_flg_ativo = 1 AND pfu.pfu_flg_ativo = 1 AND pfu.per_idt IN @perfis",
                        parameters: new { perfis },
                        cancellationToken: cancellationToken
                    )
                )
            ).ToCollection();
        }

        public async Task<TryException<IEnumerable<UsuarioModel>>> ListarUsuariosPorPerfil(Perfil perfil, CancellationToken cancellationToken) => (
            await _db.Connection.QueryAsync<UsuarioModel>(
                new CommandDefinition(
                    commandText: $@"SELECT 
                                        usp.usp_guid_ad [ActiveDirectoryId],
                                        usp.usp_nome_ad [Nome],
                                        usp.usp_email_ad [Email]
                                    FROM
                                        tb_usp_usuario_perfil usp
                                    WHERE 
                                        usp.per_idt = @perfil and
                                        usp.usp_flg_ativo = 1",
                    cancellationToken: cancellationToken,
                    transaction: Transaction,
                     parameters: new { perfil = perfil.ToInt32()}

                ))).ToCollection();
    }
}
