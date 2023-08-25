using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI1548.Consulta
{
    [ApiController]
    [AuthorizeBearer(Roles = "fi1548:consultar")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentoFI1548/consulta/liquidar")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultaLiquidarController : ControllerBase
    {
        private readonly IAuthorizationService _authService;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ConsultaLiquidarController(
            IAuthorizationService authService,
            IDocumentoFI1548AppService documentoFI1548AppService,
            IWebHostEnvironment hostingEnvironment
            )
        {
            _authService = authService;
            _documentoFI1548AppService = documentoFI1548AppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("")]
        [ApiExplorerSettings(GroupName = "Documento FI-1548")]
        public async Task<ActionResult<DocumentoFI1548Model>> Post(
            DocumentoFI1548Model model,
            CancellationToken cancellationToken = default
        )
        {
            bool permissaoLiquidar = await this.HasRoleAsync(_authService, "fi1548:consultar:liquidar");

            var response = await _documentoFI1548AppService.Liquidar(model, permissaoLiquidar, this.ObterUploadBasePath(_hostingEnvironment), cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return this.Success(response.Success);
        }
    }
}
