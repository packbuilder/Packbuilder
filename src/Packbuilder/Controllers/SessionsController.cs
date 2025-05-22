using Microsoft.AspNetCore.Mvc;
using Packbuilder.Dto;
using Microsoft.AspNetCore.Authorization;
using Packbuilder.Interfaces;
using Packbuilder.Exceptions;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Authorize]
    [Route("sessions")]
    public class SessionsController(ISessionService sessionService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> Create(CreateSessionDto sessionDto)
        {
            try
            {
                return await sessionService.CreateSession(sessionDto);
            }
            catch (BaseException)
            {
                return Unauthorized();
            }
        }
    }
    
}