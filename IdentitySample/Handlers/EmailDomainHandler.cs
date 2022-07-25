using IdentitySample.AuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentitySample.Handlers
{
    public partial class EmailDomainHandler : AuthorizationHandler<InternalUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InternalUserRequirement requirement)
        {
            var email = context.User.Identity.Name;
            var domain = email.Split('@')[1];
            if (domain.Equals("gmail.com"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
