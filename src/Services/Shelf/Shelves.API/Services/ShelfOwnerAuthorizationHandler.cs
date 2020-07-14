using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Services
{
    /// <summary>
    /// Represents resource-based authorization. The resource is id of the item owner.
    /// </summary>
    public class ShelfOwnerAuthorizationHandler : AuthorizationHandler<IsOwnerRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            IsOwnerRequirement requirement, string ownerId)
        {
            if (context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == ownerId)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }

    public class IsOwnerRequirement : IAuthorizationRequirement
    { }
}
