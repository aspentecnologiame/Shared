using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtensions
    {
        public static async Task<bool> HasRoleAsync(this ControllerBase controllerBase, IAuthorizationService authService, string role)
             => (await authService.AuthorizeAsync(controllerBase.User, role, "HasRole")).Succeeded;

        public static UsuarioModel ObterUsuario(this ControllerBase controllerBase)
        {
            if (!controllerBase.User.Identity.IsAuthenticated)
                return null;

            return new UsuarioModel
            {
                Nome = controllerBase.User.Identity.Name,
                UsuarioDeRede = controllerBase.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "",
                ActiveDirectoryId = new Guid(controllerBase.User.Claims?.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ?? "")
            };
        }

        public static bool ValidarStringSeTemformatoJson(this ControllerBase controllerBase, string texto)
        {
            try
            {
                JObject.Parse(texto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static OkResult Success(this ControllerBase controllerBase)
        {
            return controllerBase.Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controllerBase"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static OkObjectResult Success<T>(this ControllerBase controllerBase, T content)
        {
            return controllerBase.Ok(content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerBase"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static ObjectResult Failure(this ControllerBase controllerBase, ExceptionReadOnlyCollection errors)
        {
            return controllerBase.StatusCode((int)HttpStatusCode.BadRequest,
                new ResponseError
                {
                    Errors = errors.ConvertAll(exception =>
                    {

                        if (exception is BusinessException business)
                        {
                            return new Error
                            {
                                Code = business.Code,
                                Message = exception.Message
                            };
                        }

                        return new Error
                        {
                            Code = exception.GetType().Name,
                            Message = exception.Message
                        };
                    })
                }
            );
        }

        public static string ObterUploadBasePath(this ControllerBase controllerBase, IWebHostEnvironment hostingEnvironment)
        {
            if (string.IsNullOrWhiteSpace(hostingEnvironment.WebRootPath))
            {
                hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            return Path.Combine(hostingEnvironment.WebRootPath, "uploads", "temp");
        }
    }
}
