using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Domain.ExternalServices.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories
{
    public interface IEmailRepository : IRepository
    {
        Task<TryException<Return>> Enviar(EmailModel emailModel, string mensagem);
    }
}
