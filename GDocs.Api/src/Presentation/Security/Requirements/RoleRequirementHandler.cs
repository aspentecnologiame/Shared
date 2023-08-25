using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ICE.GDocs.Api.Security
{
    public class RoleRequirementHandler :
        AuthorizationHandler<RoleRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement,
            string resource)
        {
            if (context.User.IsInRole(resource))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
