using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto.Update;
using Packbuilder.Exceptions;
using Packbuilder.Interfaces;
using Packbuilder.Jobs;
using Packbuilder.Models;
using Packbuilder.RateLimits;
using Packbuilder.Services;
using StackExchange.Redis;

namespace Packbuilder.Controllers.AuthControllers
{
    [ApiController]
    [Route("/email-reset")]
    public class EmailResetController(ISessionService sessionService, IUserService userService, IConnectionMultiplexer redis) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ProfileWrite)]
        [Authorize]
        [HttpPost()]
        [EndpointName("ChangeEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] ResetEmailDto body)
        {
            User? curUser = await sessionService.GetCurrentUser();

            if(curUser is null)
            {
                return Unauthorized();
            }

            await userService.ResetUserEmail(body, curUser.Id);

            await redis.GetSubscriber().PublishAsync(SendVerificationEmailJob.ChannelName, curUser.Id);

            return Ok(); 
        }
    }
}