using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
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
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/[controller]")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ListarInputsController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;

        public ListarInputsController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("NaturezaOperacional")]
        public async Task<ActionResult<IEnumerable<DropDownModel>>> NaturezaOperacional(CancellationToken cancellationToken = default)
        {
            var response = await _saidaMaterialNotaFiscalAppService.ListarNatureza(cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var listaNatureza = response.Success;

            if (listaNatureza == null)
                return NotFound();

            return this.Success(listaNatureza);

        }


        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("ModalidadeFrete")]
        public async Task<ActionResult<IEnumerable<DropDownModel>>> ModalidadeFrete(CancellationToken cancellationToken = default)
        {
            
            var response = await _saidaMaterialNotaFiscalAppService.ListarModalidadeFrete(cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var listarModalidade = response.Success;

            if (listarModalidade == null)
                return NotFound();

            return this.Success(listarModalidade);

        }


        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Status")]
        public async Task<ActionResult<IEnumerable<DropDownModel>>> Status(CancellationToken cancellationToken = default)
        {
            var response = await _saidaMaterialNotaFiscalAppService.ListarStatusMaterial(cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            var listarModalidade = response.Success;

            if (listarModalidade == null)
                return NotFound();

            return this.Success(listarModalidade);

        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("FiltroPadraoStatusPerfil")]
        public async Task<ActionResult<IEnumerable<DropDownModel>>> FiltroPadraoStatusPerfil(CancellationToken cancellationToken = default)
        {
            var response = await _saidaMaterialNotaFiscalAppService.ListarStatusMaterialFiltro(this.ObterUsuario(), cancellationToken);



            if (response.IsFailure)
                return this.Failure(response.Failure);

            var listarModalidade = response.Success;

            if (listarModalidade == null)
                return NotFound();

            return this.Success(listarModalidade);

        }

    }
}
