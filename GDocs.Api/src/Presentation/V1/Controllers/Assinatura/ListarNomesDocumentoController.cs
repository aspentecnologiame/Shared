using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
    public class ListarNomesDocumentoController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;

        public ListarNomesDocumentoController(
            IAssinaturaAppService assinaturaAppService
            )
        {
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{termo}")]
        public async Task<ActionResult<AssinaturaNomeDocumento>> Get(
            string termo,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaAppService.ListarNomesDocumentos(termo, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var nomeList = response.Success;

            if (nomeList == null)
                return NotFound();

            return this.Success(nomeList);
        }
    }
}