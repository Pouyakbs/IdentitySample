using IdentitySample.AuthorizationRequirement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IdentitySample.Handlers
{
    public class IPAddressHandler : AuthorizationHandler<IPRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public IPAddressHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IPRequirement requirement)
        {
            var authFilterCtx = httpContextAccessor.HttpContext;
            var ipAddress = "192.168.1.105";

            List<string> whiteListIPList = requirement.Whitelist;
            var isInwhiteListIPList = whiteListIPList
                .Where(a => IPAddress.Parse(a)
                .Equals(ipAddress))
                .Any();
            foreach (var item in whiteListIPList)
            {
                if (item != ipAddress)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
