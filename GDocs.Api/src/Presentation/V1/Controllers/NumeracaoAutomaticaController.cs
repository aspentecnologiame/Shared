using ICE.GDocs.Api.Security;
using ICE.GDocs.Application;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class NumeracaoAutomaticaController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISequencialService _sequencialService;
        private readonly IConfiguracaoAppService _configuracaoAppService;

        public NumeracaoAutomaticaController(
            IWebHostEnvironment hostingEnvironment,
            ILogger<UploadController> logger,
            IConfiguration configuration,
            ISequencialService sequencialService,
            IConfiguracaoAppService configuracaoAppService
            )
        {
            _logger = logger;
            _configuration = configuration;
            _sequencialService = sequencialService;
            _configuracaoAppService = configuracaoAppService;

            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }
        }

        [ApiExplorerSettings(GroupName = "NumeracaoAutomatica")]
        [AuthorizeBearer(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<int>> NumeracaoAutomatica(
            int categoriaId,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var numeracaoAutomaticaInfo = await GerarNumeracaoAutomatica(categoriaId, cancellationToken);

                if (numeracaoAutomaticaInfo.IsFailure)
                    return this.Failure(numeracaoAutomaticaInfo.Failure);

				return this.Success(numeracaoAutomaticaInfo.Success);
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<TryException<int>> GerarNumeracaoAutomatica(int categoriaId, CancellationToken cancellationToken)
		{
            var configCategoriaInfo = await ObterConfiguracaoDaCategoria(categoriaId, cancellationToken);

            if (configCategoriaInfo.IsFailure)
                return configCategoriaInfo.Failure;

            var configCategorias = configCategoriaInfo.Success;

            if (configCategorias == null)
                return new BusinessException("configuracao-categoria-nao-encontrada", "Não foi encontrada configurações para a categoria informada.");

            if (!configCategorias.NumeracaoAutomatica.Habilitado)
                return new BusinessException("configuracao-categoria-nao-encontrada", "Categoria informada não permite gerar numeração de forma automática.");

			var chaveNumeroDocumentoAutomatico = configCategorias.NumeracaoAutomatica.ChaveSequence;

			if (string.IsNullOrEmpty(chaveNumeroDocumentoAutomatico))
				return new BusinessException("numero-documento-automatico", "Chave da numeração automática do documento não informada.");

			var sequencial = await _sequencialService.ObterProximoSequencialPorChaveCategoria(chaveNumeroDocumentoAutomatico);
			if (sequencial.IsFailure)
				return sequencial.Failure;

			if (sequencial.Success == -1)
				return new BusinessException("numero-documento-automatico", $"Não foi encontrada numeração automática '(sequence: {chaveNumeroDocumentoAutomatico})' para a categoria do documento informada.");

			return sequencial.Success;
        }

        private async Task<TryException<ConfiguracaoCategoriaModel>> ObterConfiguracaoDaCategoria(int categoriaId, CancellationToken cancellationToken)
        {
            var configuracaoCategoria = await _configuracaoAppService.ObterConfiguracaoRepositorio(_configuration.GetValue("ChaveConfiguracao:Categoria", "configCategorias"), cancellationToken);
            if (configuracaoCategoria.IsFailure)
                return configuracaoCategoria.Failure;

            var configCategorias = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfiguracaoCategoriaModel>>(configuracaoCategoria.Success.Valor);

            return configCategorias.Find(w => w.Codigo == categoriaId);
        }
    }
}
