using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class CadastroController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;

        public CadastroController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService,
                                   ISequencialService sequencialService, ILogger<CadastroController> logger)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:acao:adiciona", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<IActionResult> Post(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken = default)
        {


            saidaMaterialNotaFiscalModel.GuidAutor = this.ObterUsuario().ActiveDirectoryId;

            var response = await _saidaMaterialNotaFiscalAppService.Inserir(saidaMaterialNotaFiscalModel, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            return Ok(response.Success);

        }


    }
}
