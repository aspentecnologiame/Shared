using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SaidaMaterialNotaFiscal.Consulta
{
     
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/SaidaMaterialNotaFiscal/consulta")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultarFiltrarController : ControllerBase
    {
        private readonly ISaidaMaterialNotaFiscalAppService _saidaMaterialNotaFiscalAppService;
        private readonly IAuthorizationService _authService;


        public ConsultarFiltrarController(ISaidaMaterialNotaFiscalAppService saidaMaterialNotaFiscalAppService, IAuthorizationService authService)
        {
            _saidaMaterialNotaFiscalAppService = saidaMaterialNotaFiscalAppService;
            _authService = authService;
        }

        [ApiExplorerSettings(GroupName = "Saida de Material com Nota Fiscal")]
        [AuthorizeBearer(Roles = "SaidaMaterialNF:consulta", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("")]
        public async Task<ActionResult<IEnumerable<SaidaMaterialNotaFiscalModel>>> Post(SaidaMaterialNotaFiscalFilterModel filtro, CancellationToken cancellationToken = default)
        {
            filtro.Autores = await this.HasRoleAsync(_authService, "SaidaMaterialNF:consulta:todosusuario")
               ? filtro.Autores : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };

            filtro.UsuarioLogadoAd = this.ObterUsuario().ActiveDirectoryId;

            var response = await _saidaMaterialNotaFiscalAppService.ConsultaMaterialNotaFiscalPorFiltro(filtro, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();

            return this.Success(response.Success);

        }
    }
}
