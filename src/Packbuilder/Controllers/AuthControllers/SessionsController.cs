using Microsoft.AspNetCore.Mvc;
using Packbuilder.Interfaces;
using Packbuilder.Exceptions;
using Packbuilder.Dto.Create;
using Packbuilder.Models;
using Microsoft.AspNetCore.Authorization;
using Packbuilder.Attributes;
using Packbuilder.RateLimits;
using Packbuilder.Dto;

namespace Packbuilder.Controllers.AuthControllers
{
    [ApiController]
    [Route("sessions")]
    public class SessionsController(ISessionService sessionService) : ControllerBase
    {   
        [RateLimit(RateLimitBuckets.General)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<CreateUserDto>> GetUserData()
        {
            User? curUser = await sessionService.GetCurrentUser();

            if(curUser is null)
            {
                return Unauthorized();
            }

            return Ok(new UserDto
            {
                Name = curUser.Name,
                Id = curUser.Id,
                Email = curUser.Email,
                EmailVerified = curUser.EmailVerified,
                ImageType = curUser.ImageType,
                ImageValue = curUser.ImageValue
            });
        }
        
        [RateLimit(RateLimitBuckets.Auth)]
        [HttpPost]
        [EndpointName("Login")]
        public async Task<ActionResult> Create(CreateSessionDto sessionDto)
        {
            try
            {
                await sessionService.CreateSession(sessionDto);

                return Ok();
            }
            catch (ExceptionBase)
            {
                return Unauthorized();
            }
        }

        [RateLimit(RateLimitBuckets.General)]
        [Authorize]
        [HttpDelete]
        [EndpointName("Logout")]
        public async Task<ActionResult> Delete()
        {
            try
            {
                await sessionService.DeleteSession();

                return Ok();
            }
            catch (ExceptionBase)
            {
                return BadRequest();
            }
        }
        
        [RateLimit(RateLimitBuckets.Auth)]
        [Authorize]
        [HttpPost("/refresh")]
        [EndpointName("RefreshSession")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            try
            {
                return await sessionService.RefreshSession();
            }
            catch (ExceptionBase)
            {
                return Unauthorized();
            }
        }
    }
    
}