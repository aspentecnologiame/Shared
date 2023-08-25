using ICE.GDocs.Api.Security;
using ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.V1.Controllers.SolicitacaoSaidaMaterial.Consulta
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/DocumentoFI347/consulta")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ResponseError), (int)HttpStatusCode.InternalServerError)]
    public class ConsultaFiltrarController : ControllerBase
    {
        private readonly IAuthorizationService _authService;
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;

        public ConsultaFiltrarController(
            IAuthorizationService authService,
            ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService
            )
        {
            _authService = authService;
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
        }

        [ApiExplorerSettings(GroupName = "Documento FI-347")]
        [AuthorizeBearer(Roles = "fi347:consultar", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("filtrar")]
        public async Task<ActionResult<IEnumerable<SolicitacaoSaidaMaterialModel>>> Post(
            SolicitacaoSaidaMaterialFilterModel filtro,
            CancellationToken cancellationToken = default
        )
        {
			filtro.Autores = await this.HasRoleAsync(_authService, "fi347:consultar:todosusuarios")
				? filtro.Autores : new List<Guid> { this.ObterUsuario().ActiveDirectoryId };

			filtro.UsuarioLogadoAd = this.ObterUsuario().ActiveDirectoryId;

            var response = await _solicitacaoSaidaMaterialAppService.ConsultarSolicitacoesDeSaidaMateriaisPorFiltro(filtro, cancellationToken);

            if (response.IsFailure)
                return this.Failure(response.Failure);

            if (response.Success == null)
                return NotFound();          

            return this.Success(response.Success);
        }
    }
}
