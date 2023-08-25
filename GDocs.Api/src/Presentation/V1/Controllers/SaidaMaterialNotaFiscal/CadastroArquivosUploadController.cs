using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class CadastroArquivosUploadController : ControllerBase
    {

        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public CadastroArquivosUploadController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService, IWebHostEnvironment hostingEnvironment)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:retorno:uploadNf,SaidaMaterialNF:acao:uploadNotaSaida,SaidaMaterialNF:acao:trocarNotaFiscal", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<Return>> Post(
            IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel,
            CancellationToken cancellationToken = default
            )
        {

            var result = await _saidaMaterialNotaFiscalAppService.SalvarArquivosUpload(saidaMaterialArquivoModel, this.ObterUploadBasePath(_hostingEnvironment),
                                                                                                                  this.ObterUsuario().ActiveDirectoryId, cancellationToken);

            if (result.IsFailure)
                return this.Failure(result.Failure);

            return Return.Empty;

        }
    }
}
