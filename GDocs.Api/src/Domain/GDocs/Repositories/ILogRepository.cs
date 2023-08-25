using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Domain.Models.Trace;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories
{
    public interface ILogRepository : IRepository
    {
        Task<long> Inserir(LogModel logModel);

        Task<Guid> InserirDadosRequisicao(DadosRequisicao dadosRequisicao);
    }
}
