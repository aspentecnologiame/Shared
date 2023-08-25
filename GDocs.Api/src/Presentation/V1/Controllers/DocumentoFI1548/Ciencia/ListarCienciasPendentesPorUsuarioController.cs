using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Ciencia
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI1548/ciencia/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarCienciasPendentesPorUsuarioController : ControllerBase
    {

        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public ListarCienciasPendentesPorUsuarioController(IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _documentoFI1548AppService = documentoFI1548AppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        [AuthorizeBearer(Roles = "fi1548:ciencia:pendente", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<ProcessoAssinaturaDocumentoModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProcessoAssinaturaDocumentoModel>>> Get(
        CancellationToken cancellationToken = default)
        {
            var usuario = this.ObterUsuario();

            var listaCiencias = await _documentoFI1548AppService.ListarCienciasPendentesDeAprovacaoPorUsuario(usuario.ActiveDirectoryId, cancellationToken);

            if (listaCiencias.IsFailure)
                return this.Failure(listaCiencias.Failure);

            return this.Success(listaCiencias.Success);
        }


    }
}
