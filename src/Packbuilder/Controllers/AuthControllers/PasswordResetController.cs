using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Jobs;
using Packbuilder.Models;
using Packbuilder.RateLimits;
using StackExchange.Redis;

namespace Packbuilder.Controllers.AuthControllers
{
    [ApiController]
    [Route("/password-reset")]
    public class PasswordResetController(PackbuilderContext context, IUserService userService, IConnectionMultiplexer redis) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.Auth)]
        [HttpPost("request/{email}")]
        [EndpointName("sendPasswordResetEmail")]
        public async Task<ActionResult> SendPasswordResetEmail([FromRoute] string email)
        {
            User? existingUser = await context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if(existingUser is null)
            {
                return BadRequest();
            }

            await redis.GetSubscriber().PublishAsync(SendPasswordResetEmailJob.ChannelName, existingUser.Id);

            return Ok();
        }

        [RateLimit(RateLimitBuckets.ProfileWrite)]
        [HttpPost("{userId}")]
        [EndpointName("resetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto body, [FromRoute] int userId)
        {
            User? existingUser = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if(existingUser is null)
            {
                return BadRequest();
            }

            bool success = await userService.ResetUserPassword(body.Token, existingUser.Id, body.NewPassword);

            if(!success)
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}