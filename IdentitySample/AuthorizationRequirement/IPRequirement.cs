using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySample.AuthorizationRequirement
{
    public class IPRequirement : IAuthorizationRequirement
    {
        public List<string> Whitelist { get; }
        public IPRequirement(ApplicationOptions applicationOptions)
        {
            Whitelist = applicationOptions.WhiteList;
        }
    }
}
