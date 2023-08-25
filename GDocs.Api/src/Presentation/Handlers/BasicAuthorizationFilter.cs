using ICE.Framework.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ICE.GDocs.Api.Handlers
{
    internal class BasicAuthorizationFilterAttribute : ActionFilterAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public BasicAuthorizationFilterAttribute(
           IConfiguration configuration,
           ILogger<BasicAuthorizationFilterAttribute> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Esse método foi deixado em branco intencionalmente
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var result = ValidateAuthorizationHeaderValue(context.HttpContext.Request.Headers["Authorization"]);

                if (result.StatusCode != (int)HttpStatusCode.OK)
                    context.Result = result;
            }
            else
            {
                context.Result = context.Result = new ObjectResult(default)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };


                var headers = string.Join(",", context.HttpContext.Request.Headers?.Keys?.Select(k => k)?.ToArray() ?? new List<string>().ToArray());

                _logger.LogInformation($"Erro BAD_REQUEST_HEADER_AUTH_MISSING. HEADERS: {headers}");
            }

            base.OnActionExecuting(context);
        }

        private ObjectResult ValidateAuthorizationHeaderValue(string authorizationHeaderValues)
        {
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(authorizationHeaderValues);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var usuario = credentials[0];
                var senha = HashingSHA1ManagedHelper.GenerateHash(credentials[1]);
                var usuarioToCompare = _configuration.GetValue<string>("UsuarioBasicAuth");
                var senhaToCompare = _configuration.GetValue<string>("SenhaBasicAuth");

                if (usuario != usuarioToCompare.Trim() || senha != senhaToCompare.Trim())
                    return new ObjectResult(default)
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                    };
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Erro INTERNAL_SERVER_ERROR. HEADERS: {ex.Message}");

                return new ObjectResult(ex.Message)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                };
            }

            return new ObjectResult(default)
            {
                StatusCode = (int)HttpStatusCode.OK,
            };

        }
    }
}