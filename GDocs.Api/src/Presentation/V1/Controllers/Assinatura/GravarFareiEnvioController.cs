using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Linq;
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
    public class GravarFareiEnvioController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public GravarFareiEnvioController(
            IAssinaturaAppService assinaturaAppService)
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [HttpPost("")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Return>> Post(
            int padId,
            bool fareiEnvio,
            CancellationToken cancellationToken = default
        )
        {
            var responsePassoUsuario = await _assinaturaAppService.ObterProcessoComPassosParaAssinatura(padId, cancellationToken);
            if (responsePassoUsuario.IsFailure)
                return this.Failure(responsePassoUsuario.Failure);

            var responseFareiEnvio = await _assinaturaAppService.SalvarFareiEnvio(responsePassoUsuario.Success,this.ObterUsuario().ActiveDirectoryId ,fareiEnvio, cancellationToken);

            if (responseFareiEnvio.IsFailure)
                return this.Failure(responseFareiEnvio.Failure);

            return Return.Empty;
        }
    }
}