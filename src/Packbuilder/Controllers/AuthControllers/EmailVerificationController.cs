using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Packbuilder.Attributes;
using Packbuilder.Config;
using Packbuilder.Interfaces;
using Packbuilder.Jobs;
using Packbuilder.Models;
using Packbuilder.RateLimits;
using StackExchange.Redis;

namespace Packbuilder.Controllers.AuthControllers
{
    [ApiController]
    [Route("/verification")]
    public class EmailVerificationController(PackbuilderContext context, IUserService userService, IConnectionMultiplexer redis, IOptions<AppSettings> appSettings) : ControllerBase
    {
        [HttpGet]
        [EndpointName("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string verificationCode, [FromQuery] int userId)
        {   
            try
            {
                bool success = await userService.VerifyUserEmail(verificationCode,userId);

                return Redirect($"{appSettings.Value.FrontendBaseUrl}/profile/verification-result?status={(success ? "success" : "fail")}");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return Redirect($"{appSettings.Value.FrontendBaseUrl}/profile/verification-result?status=fail");
            }
        }

        [RateLimit(RateLimitBuckets.Auth)]
        [HttpPost("request/{email}")]
        [EndpointName("sendVerificationEmail")]
        public async Task<ActionResult> SendVerificationEmail([FromRoute] string email)
        {
            User? existingUser = await context.Users.SingleOrDefaultAsync(u => u.Email == email);

            if(existingUser is null || existingUser.EmailVerified == true)
            {
                return BadRequest();
            }

            await redis.GetSubscriber().PublishAsync(SendVerificationEmailJob.ChannelName, existingUser.Id);

            return Ok();
        }
    }
}