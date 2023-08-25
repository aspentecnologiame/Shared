using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Models.Trace;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    public interface ILogService : IDomainService
    {
        Task<TryException<long>> RegistrarLog(LogModel log);

        Task<TryException<Guid>> InserirDadosRequisicao(DadosRequisicao dadosRequisicao);        
        Task<TryException<Return>> AdicionarRastreabilidade(dynamic metadados, string texto);
    }
}
