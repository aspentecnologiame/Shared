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
    internal class TemplateProcessoAssinaturaDocumentoRepository : Repository, ITemplateProcessoAssinaturaDocumentoRepository
    {
        public TemplateProcessoAssinaturaDocumentoRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ListarPassosEUsuariosPorTag(string tag, CancellationToken cancellationToken)
        {
            var passos = new Dictionary<int, AssinaturaPassoItemModel>();

            return (await _db.Connection.QueryAsync<AssinaturaPassoItemModel, AssinaturaUsuarioModel, AssinaturaPassoItemModel>(
                new CommandDefinition(
                    commandText: @"
                SELECT 	
	                tpadp.tpadp_idt AS Id,
	                tpadp.tpadp_ordem AS Ordem,
	                tpadp.tpadp_flg_aguardar_todos_usuarios AS AguardarTodosUsuarios,

	                tpadpu.tpadp_idt AS Id,
	                tpadpu.tpadpu_guid_ad AS [Guid],
	                tpadpu.tpadpu_flg_notificar_finalizacao AS NotificarFinalizacao,
                    tpadpu.tpadpu_notificar_finalizacao_fluxo AS FluxoNotificacao
                FROM 
	                tb_tpad_template_processo_assinatura_documento tpad
                INNER JOIN 	tb_tpadp_template_processo_assinatura_documento_passo tpadp 
	                ON tpadp.tpad_idt = tpad.tpad_idt
                INNER JOIN	tb_tpadpu_template_processo_assinatura_documento_passo_usuario tpadpu
	                ON tpadpu.tpadp_idt = tpadp.tpadp_idt
                INNER JOIN 	tb_tpadt_template_processo_assinatura_documento_tag tpadt
	                ON tpadt.tpad_idt = tpad.tpad_idt 
	                AND tpadt.tpadt_flg_ativo=1
                WHERE
	                tpad.tpad_flg_ativo =1 AND
	                tpadp.tpadp_flg_ativo =1 AND
	                tpadpu.tpadpu_flg_ativo = 1 AND
                    tpadt.tpadt_tag = @tag                                  
                ORDER BY  
                    tpadp.tpadp_ordem ",
                    parameters: new
                    {
                        tag
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                ),
                splitOn: "Id,Id",
                map: (passoMap, usuarioMap) =>
                {
                    if (!passos.TryGetValue(passoMap.Id, out var passo))
                    {
                        passo = passoMap;
                        passos.Add(passo.Id, passo);
                    }

                    if (usuarioMap != null && passo.Usuarios.Empty(u => u.Guid == usuarioMap.Guid))
                    {
                        passo.Usuarios.Add(usuarioMap);
                    }

                    return passo;
                }
                )).Distinct()
                .ToCollection();
        }
    }
}