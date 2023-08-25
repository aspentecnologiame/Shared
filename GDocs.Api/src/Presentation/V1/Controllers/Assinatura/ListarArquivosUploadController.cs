using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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
    public class ListarArquivosUploadController : ControllerBase
    {
        
        private readonly IAssinaturaAppService _assinaturaAppService;


        public ListarArquivosUploadController( IAssinaturaAppService assinaturaAppService)
        {
           
            _assinaturaAppService = assinaturaAppService;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,assinatura:gerenciardocumentos:todosusuarios,assinatura:pendencias", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{processoAssinaturaDocumentoId}")]
        [ProducesResponseType(typeof(IEnumerable<AssinaturaArquivoModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<AssinaturaArquivoModel>>> Get(
            int processoAssinaturaDocumentoId,
            bool listarTodos = true,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _assinaturaAppService.ListarArquivoExpurgoPorPad(processoAssinaturaDocumentoId, cancellationToken, listarTodos);
            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        }
    }
}