using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Models.Trace;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.Data.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories
{
    internal class LogRepository : Repository, ILogRepository
    {       
        public LogRepository(            
            IGDocsDatabase db,
            IUnitOfWork unitOfWork
        ) : base(db, unitOfWork)
        {            
        }

        public async Task<long> Inserir(LogModel logModel)
            => await _db.Connection.QueryFirstAsync<long>(
                @"INSERT tb_log_log (tpl_idt, log_metadados, log_dados)
				VALUES (@tipoLogId, @metadados, @dados);
				SELECT CAST(SCOPE_IDENTITY() as bigint)",
                new
                {
                    tipoLogId = logModel.Tipo.ToInt32(),
                    metadados = logModel.Metadados.ToString(),
                    dados = logModel.Dados
                },
                transaction: Transaction
            );

        public async Task<Guid> InserirDadosRequisicao(DadosRequisicao dadosRequisicao)
        {
            const int LengthStatus = 50;
            var metadados = dadosRequisicao.Metadados.Dados as IDictionary<string, object>;
            var status = metadados["Status"].ToString();
            if (status.Length > LengthStatus)
                status = status.Substring(0, LengthStatus);

            var rastreabilidadeId = Guid.NewGuid();

            await _db.Connection.ExecuteAsync(@"
                INSERT INTO [dbo].[tb_lre_log_requisicao]
                    (lre_idt,lre_origem,lre_recurso,lre_status,lre_metadados,lre_xml_requisicao)
                VALUES
                    (@rastreabilidadeId, @origem, @recurso, @status, @metadadosXml, @conteudoXml)",
                 new
                 {
                     rastreabilidadeId,
                     origem = metadados["Origem"],
                     recurso = metadados["Recurso"],
                     status,
                     metadadosXml = dadosRequisicao.Metadados.DadosXml.InnerXml,
                     conteudoXml = dadosRequisicao.Conteudo.DadosXml.InnerXml                     
                 }
             );

            return rastreabilidadeId;
        } 
    }
}
