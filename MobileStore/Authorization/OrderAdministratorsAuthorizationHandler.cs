using System.Threading.Tasks;
using MobileStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Azure.KeyVault.Models;

namespace MobileStore.Authorization
{
    public class OrderAdministratorsAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, BaseModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            BaseModel resource)
        {
            if (context.User == null)
            {
                return Task.FromResult(0);
            }

            // Administrators can do anything.
            if (context.User.IsInRole(Constants.AdminRole))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}