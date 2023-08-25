using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.Assinatura
{
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/Assinatura/[controller]")]
    [ApiController]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarArquivoProcessoCompletoController : ControllerBase
    {
        private readonly IArquivoRepository _arquivoRepository;
        private readonly IAssinaturaAppService _assinaturaAppService;


        public ListarArquivoProcessoCompletoController(IArquivoRepository arquivoRepository, IAssinaturaAppService assinaturaAppService)
        {
            _arquivoRepository = arquivoRepository;
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:gerenciardocumentos:todosusuarios,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{padId}")]
        [ProducesResponseType(typeof(IEnumerable<Return>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Return>> Get(
            int padId,
            CancellationToken cancellationToken = default
        )
        {
            var responseVerificaProcesso = await _assinaturaAppService.VerificarProcessoCompleto(padId, this.ObterUsuario().ActiveDirectoryId, cancellationToken);
            if (responseVerificaProcesso.IsFailure)
                return this.Failure(responseVerificaProcesso.Failure);

            if (!responseVerificaProcesso.Success)
                return Return.Empty;

            var response = await _arquivoRepository.ListarArquivosUploadPorPadId(padId, cancellationToken);
            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        }
    }
}