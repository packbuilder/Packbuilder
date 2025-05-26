using Microsoft.AspNetCore.Authorization;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Policys;

public class EmailVerifiedHandler(ISessionService sessionService) : AuthorizationHandler<EmailVerifiedRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailVerifiedRequirement requirement)
    {
        User? user = await sessionService.GetCurrentUser();

        if(user is null || user.EmailVerified is false)
        {
            return;
        }

        context.Succeed(requirement);
    }
}