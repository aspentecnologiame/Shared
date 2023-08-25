using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Threading;

namespace ICE.GDocs.Api.V1.Controllers.DocumentoFI347.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/documentoFI347/consulta/ItemMaterial")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultarItemMaterialController : ControllerBase
    {

        private readonly ISolicitacaoSaidaMaterialItemAppService _solicitacaoSaidaMaterialItemAppService;

        public ConsultarItemMaterialController(ISolicitacaoSaidaMaterialItemAppService solicitacaoSaidaMaterialItemAppService)
        {
            _solicitacaoSaidaMaterialItemAppService = solicitacaoSaidaMaterialItemAppService;
        }



        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]

        public async Task<ActionResult<IEnumerable<SolicitacaoSaidaMaterialItemModel>>> Get(
            int idSaidaMaterial,
            int acao,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _solicitacaoSaidaMaterialItemAppService.ObterPorIdSolicitacaoSaidaMaterial(idSaidaMaterial, acao, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);
        } 

    }
}
