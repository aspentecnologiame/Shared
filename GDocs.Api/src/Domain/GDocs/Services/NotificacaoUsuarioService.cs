using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services
{
    internal class NotificacaoUsuarioService : DomainService, INotificacaoUsuarioService
    {
        private readonly INotificacaoUsuarioRepository _notificacaoUsuarioRepository;
        private readonly IRdlToPdfBytesConverterExternalService _rdlToPdfBytesConverterExternalService;
        public NotificacaoUsuarioService(
            IUnitOfWork unitOfWork,
            INotificacaoUsuarioRepository notificacaoUsuarioRepository,
            IRdlToPdfBytesConverterExternalService rdlToPdfBytesConverterExternalService
        ) : base(unitOfWork)
        {
            _notificacaoUsuarioRepository = notificacaoUsuarioRepository;
            _rdlToPdfBytesConverterExternalService = rdlToPdfBytesConverterExternalService;
        }

        public async Task<TryException<NotificacaoRelatorioModel>> ObterNotificacaoNaoLidaPorIdNotificacao(int idNotificacaoUsuario, CancellationToken cancellationToken)
        {
            TryException<NotificacaoRelatorioModel> result;

            result = await _notificacaoUsuarioRepository.ObterRelatorioNaoLidoPorIdNotificacao(idNotificacaoUsuario, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            var relatorio = await ObterRelatorio(cancellationToken, (RelatorioNotificacao)result.Success.IdRelatorio);
            if (relatorio.IsFailure) return await Task.FromResult(relatorio.Failure);

            result.Success.RdlBytes = relatorio.Success;

            return result;
        }

        public async Task<TryException<IEnumerable<NotificacaoRelatorioModel>>> ObterNotificacoesNaoLidasPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioRepository.ObterRelatoriosNaoLidosPorIdUsuario(idUsuario, cancellationToken);


        public async Task<TryException<int>> ObterQuantidadeNotificacoesNaoLidasPorIdUsuario(Guid idUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioRepository.ObterQuantidadeRelatoriosNaoLidosPorIdUsuario(idUsuario, cancellationToken);

        public async Task<TryException<int>> AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(int idNotificacaoUsuario, CancellationToken cancellationToken)
        => await _notificacaoUsuarioRepository.AtualizarRelatorioParaLidoPorIdNotificacaoUsuario(idNotificacaoUsuario, cancellationToken);

        private async Task<TryException<byte[]>> ObterRelatorio(CancellationToken cancellationToken, RelatorioNotificacao relatorioNotificacao)
        {
            var parametrosRequest = new Dictionary<string, string>();
            var relatorio = await _notificacaoUsuarioRepository.ObterParametrosDoRelatorio(cancellationToken, relatorioNotificacao);

            if (relatorio.IsFailure) return relatorio.Failure;

            var parametrosDB = relatorio.Success?.Parametros.Split('&');

            if (parametrosDB != null)
            {
                for(var i = 0; i < parametrosDB.Count(); i++)
                {
                    var descricaoEValor = parametrosDB[i].Split('=');
                    parametrosRequest.Add(descricaoEValor[0], descricaoEValor[1]);
                }
            }

            var responseBinario = _rdlToPdfBytesConverterExternalService.Converter(relatorio.Success.Url, parametrosRequest);

            if (responseBinario.IsFailure) return await Task.FromResult(responseBinario.Failure);

            return await Task.FromResult(responseBinario.Success);
        }
    }
}
