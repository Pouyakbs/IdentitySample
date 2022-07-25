using IdentitySample.AuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentitySample.Handlers
{
    public partial class EmailDomainHandler
    {
        public class ExcludeContractorHandler : AuthorizationHandler<InternalUserRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InternalUserRequirement requirement)
            {
                var roles = ((ClaimsIdentity)context.User.Identity).Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);
                if (!roles.Contains("User"))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }
    }
}
