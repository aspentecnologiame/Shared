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
    internal class AssinaturaCategoriaRepository : Repository, IAssinaturaCategoriaRepository
    {
        public AssinaturaCategoriaRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<IEnumerable<AssinaturaCategoriaModel>>> Listar(IEnumerable<int> ListaIdsExclude = default, CancellationToken cancellationToken = default)
        {
            var result = await _db.Connection.QueryAsync<AssinaturaCategoriaModel>(
                new CommandDefinition(
                    commandText: $@"SELECT 
                                        padc.[padc_idt] Id,
	                                    padc.[padc_descricao] Descricao
                                    FROM 	
	                                    [dbo].[tb_padc_processo_assinatura_documento_categoria] padc
                                    WHERE
	                                    padc.[padc_flg_ativo] = 1                                        
                                        AND (@countExclude = 0 OR padc.[padc_idt] NOT IN @listaIdsExclude)
                                    ORDER BY
	                                    padc.[padc_descricao]",
                    parameters: new
                    {

                        countExclude = ListaIdsExclude?.Count() ?? 0,
                        listaIdsExclude = ListaIdsExclude ?? new List<int>()
                    },
                    cancellationToken: cancellationToken,
                    transaction: Transaction
                )
            );

            return result?.ToCollection();
        }
    }
}
