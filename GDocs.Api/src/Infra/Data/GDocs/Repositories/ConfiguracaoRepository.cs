using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories
{
    internal class ConfiguracaoRepository : Repository, IConfiguracaoRepository
    {
        public ConfiguracaoRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<ConfiguracaoModel>> ObterConfiguracao(string chave, CancellationToken cancellationToken)
        {
            var configuracoes = await _db.Connection.QueryAsync<ConfiguracaoModel>(
                new CommandDefinition(

                    commandText: @"
					    SELECT
						    cfg_idt as [Id],
						    cfg_nom_key as [Chave],
						    cfg_des_value as [Valor],
						    cfg_des_descricao as [Descricao],
						    cfg_flg_ativo as [Ativo]
					    FROM
						    dbo.tb_cfg_configuracao
					    WHERE
						    cfg_nom_key = @chave"
                    ,
                    parameters: new { chave },
                    cancellationToken: cancellationToken
                )
            );

            return configuracoes
                .ToCollection()
                .FirstOrDefault();
        }
    }
}
