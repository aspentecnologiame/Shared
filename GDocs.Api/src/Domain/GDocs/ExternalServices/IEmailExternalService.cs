using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.ExternalServices
{
    public interface IEmailExternalService
    {
        Task<TryException<Return>> Enviar(EmailModel emailModel);

        Task<TryException<Return>> EnviarEmailPorPerfil(EmailModel emailModel, Perfil perfil, CancellationToken cancellationToken);

    }
}
