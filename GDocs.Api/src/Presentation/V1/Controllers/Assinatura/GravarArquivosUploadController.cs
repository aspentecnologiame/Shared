using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
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
    public class GravarArquivosUploadController : ControllerBase
    {
        private readonly IAssinaturaAppService _assinaturaAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GravarArquivosUploadController(IAssinaturaAppService assinaturaAppService, IWebHostEnvironment hostingEnvironment)
        {
            _assinaturaAppService = assinaturaAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [ApiExplorerSettings(GroupName = "Assinatura")]
        [AuthorizeBearer(Roles = "assinatura:gerenciardocumentos:adicionar,fi347:adicionar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<Return>> Post(
                       IEnumerable<AssinaturaArquivoModel> assinaturaArquivoModel,
            CancellationToken cancellationToken = default
        )
        {
            var gravar = await _assinaturaAppService.SalvarArquivos(assinaturaArquivoModel, this.ObterUploadBasePath(_hostingEnvironment), cancellationToken);

            if (gravar.IsFailure)
                return this.Failure(gravar.Failure);

            return Return.Empty;
        }
    }
}