using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Jobs;
using StackExchange.Redis;
using Packbuilder.Attributes;
using Packbuilder.RateLimits;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController(PackbuilderContext context, ISessionService sessionService, IUserService userService, IConnectionMultiplexer redis) : ControllerBase
    {
        [HttpGet("{id:int}")]
        [EndpointName("GetUserById")]
        public async Task<ActionResult<User>> GetUserById([FromRoute] int id)
        {
            return Ok(await context.Users.SingleOrDefaultAsync(u => u.Id == id));
        }
        
        [RateLimit(RateLimitBuckets.Auth)]
        [HttpPost]
        [EndpointName("CreateAccount")]
        public async Task<ActionResult> PostUser([FromBody] CreateUserDto body)
        {
            User? existingUsername = await context.Users.SingleOrDefaultAsync(u => u.Name == body.Name);
            User? existingEmail = await context.Users.SingleOrDefaultAsync(u => u.Email == body.Email);

            if(existingUsername is not null || existingEmail is not null)
            {
                return BadRequest();
            }

            int userId = await userService.CreateAccountAsync(body);

            await redis.GetSubscriber().PublishAsync(SendVerificationEmailJob.ChannelName, userId);

            return Ok(userId);
        }

        [RateLimit(RateLimitBuckets.ProfileWrite)]
        [Authorize]
        [HttpPut("{userId}")]
        [EndpointName("UpdateUser")]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDto body, [FromRoute] int userId)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                return NotFound();
            }

            if(currentUser is null || currentUser.Id != user?.Id)
            {
                return Unauthorized();
            }

            await userService.UpdateAccountAsync(body, userId);

            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int userId)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user is null)
            {
                return NotFound();
            }

            if (currentUser is null || currentUser.Id != user.Id)
            {
                return Unauthorized();
            }

            await userService.DeleteAccountAsync(userId);

            return Ok();
        }
    }
}

