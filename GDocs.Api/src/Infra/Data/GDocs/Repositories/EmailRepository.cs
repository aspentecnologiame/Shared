using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.GDocs.Repositories;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories
{
    internal class EmailRepository : Repository, IEmailRepository
    {
        public EmailRepository(IGDocsDatabase db, IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
        }

        public async Task<TryException<Return>> Enviar(EmailModel emailModel, string mensagem)
        {
            await _db.Connection.ExecuteScalarAsync<bool>(
                sql: @"[dbo].[spr_enviar_email]",
                param: new
                {
                    para = emailModel.Destinatario,
                    assunto = emailModel.Assunto,
                    mensagem,
                    anexo = emailModel.CaminhoArquivoAnexo
                },
                commandType: CommandType.StoredProcedure,
                transaction: Transaction
            );

            return Return.Empty;
        }
    }
}
