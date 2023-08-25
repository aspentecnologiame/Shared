using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentofi1548/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarReferenciaSubstitutosController : ControllerBase
    {
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ListarReferenciaSubstitutosController(
            IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<IEnumerable<DropDownModel>>> Post(
            CancellationToken cancellationToken = default
        )
        {
            var response = await _documentoFI1548AppService.ListarReferenciaSubstitutos(this.ObterUsuario().ActiveDirectoryId, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var statusList = response.Success;  

            if (statusList == null)
                return NotFound();

            return this.Success(statusList);
        }
    }
}