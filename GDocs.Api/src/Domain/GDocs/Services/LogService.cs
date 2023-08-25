using ICE.GDocs.Common.Core.Domain.ValueObjects;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Models.Trace;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    internal class LogService : DomainService, ILogService
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogger<LogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(
            IUnitOfWork unitOfWork,
            ILogRepository logRepository,
            ILogger<LogService> logger,
            IHttpContextAccessor httpContextAccessor
        ) : base(unitOfWork)
        {
            _logRepository = logRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TryException<Guid>> InserirDadosRequisicao(DadosRequisicao dadosRequisicao)
            => await _logRepository.InserirDadosRequisicao(dadosRequisicao);

        public async Task<TryException<long>> RegistrarLog(LogModel log)
            => await _logRepository.Inserir(log);

        public async Task<TryException<Return>> AdicionarRastreabilidade(dynamic metadados, string texto)
        {
            var nomeUsuario = _httpContextAccessor.HttpContext.User?.Identity?.Name;

            /// A exceção está sendo capturada neste momento, para a falha ao salvar o 
            /// log no banco de dados não interrompa o fluxo principal [Caio Humberto Francisco]
            try
            {
                await _logRepository.Inserir(
                     new LogModel()
                         .DefinirTipo(TipoLog.Rastreamento)
                         .DefinirMetadados(new Metadados(metadados))
                         .DefinirDados(new MensagemLog(nomeUsuario, texto)
                     )
                 );

                return Return.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar inserir rastreabilidade.", null);
                return ex;
            }
        }
    }
}
