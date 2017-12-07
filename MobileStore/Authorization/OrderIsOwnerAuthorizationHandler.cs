using System;
using System.Threading.Tasks;
using MobileStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MobileStore.Data;

namespace MobileStore.Authorization
{
    public class OrderIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Order>
    {
        UserManager<ApplicationUser> _userManager;

        public OrderIsOwnerAuthorizationHandler(UserManager<ApplicationUser>
            userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                OperationAuthorizationRequirement requirement,
                Order resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.FromResult(false);
            }

            // If we're not asking for CRUD permission, return.

            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.FromResult(false);
            }

            if (resource.ApplicationUserID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(false);
        }
    }
}
