using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Consulta
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/consulta/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ObterMaterialNotaFiscalPorIdController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;

        public ObterMaterialNotaFiscalPorIdController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }


        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<SaidaMaterialNotaFiscalModel>> Get(
            int smnfIdt,
            CancellationToken cancellationToken = default) 
        {

            var response = await _saidaMaterialNotaFiscalAppService.ObterMaterialNotaFiscalPorId(smnfIdt,cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        }
    }
}
