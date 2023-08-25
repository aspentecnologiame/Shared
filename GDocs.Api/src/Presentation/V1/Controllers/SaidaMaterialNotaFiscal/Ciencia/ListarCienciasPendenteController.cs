using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Ciencia
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/ciencia/listarcienciaspendente")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarCienciasPendenteController : ControllerBase
    {

        private readonly ISolicitacaoCienciaNotaFiscalAppService _solicitacaoCienciaNotaFiscalAppService;

        public ListarCienciasPendenteController(ISolicitacaoCienciaNotaFiscalAppService solicitacaoCienciaNotaFiscalAppService)
        {
            _solicitacaoCienciaNotaFiscalAppService = solicitacaoCienciaNotaFiscalAppService;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:ciencia:pendente", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<ProcessoAssinaturaDocumentoModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProcessoAssinaturaDocumentoModel>>> Get(
        CancellationToken cancellationToken = default)
        {
            var usuario = this.ObterUsuario();

            var listaCiencias = await _solicitacaoCienciaNotaFiscalAppService.ListarCienciasPendentesParaAprovacaoPeloUsuario(usuario.ActiveDirectoryId, cancellationToken);

            if (listaCiencias.IsFailure)
                return this.Failure(listaCiencias.Failure);

            return this.Success(listaCiencias.Success);
        }


    }
}
