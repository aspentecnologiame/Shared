using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI347
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarStatusSaidaMaterial : ControllerBase
    {
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;

        public ListarStatusSaidaMaterial(ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService)
        {
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<IEnumerable<DocumentoFI347StatusModel>>> Post(
      CancellationToken cancellationToken = default)
        {
            var response = await _solicitacaoSaidaMaterialAppService.ListarStatusMaterial(cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var statusList = response.Success;

            if (statusList == null)
                return NotFound();

            return this.Success(statusList);
        }
    }
}
