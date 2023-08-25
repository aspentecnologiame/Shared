using Microsoft.AspNetCore.Mvc;
using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/consulta/ItemMaterialNotaFiscal")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultarItemMaterialNFController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalItemAppService _saidaItemMaterialNotaFiscalAppService;

        public ConsultarItemMaterialNFController(ISaidaMaterialNotaFiscalItemAppService saidaItemMaterialNotaFiscalAppService)
        {
            _saidaItemMaterialNotaFiscalAppService = saidaItemMaterialNotaFiscalAppService;
        }


        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<SaidaMaterialNotaFiscalModel>>> Get(
            int idSaidaMaterial,
            SaidaMaterialNotaFiscalTipoAcao acao,
            CancellationToken cancellationToken = default)
        {

            var buscaItem = await _saidaItemMaterialNotaFiscalAppService.ObterPorIdSolicitacaoSaidaMaterial(idSaidaMaterial, acao, cancellationToken);

            return this.Success(buscaItem.Success);
        }
    }
}
