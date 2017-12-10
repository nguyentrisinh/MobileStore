using System.Threading.Tasks;
using MobileStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace MobileStore.Authorization
{
    public class OrderSalesAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, BaseModel>
    {
     

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, BaseModel resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.FromResult(0);
            }

            // If we're not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName)
            {
                return Task.FromResult(0);
            }

            if (context.User.IsInRole(Constants.OrderSalesRole))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}