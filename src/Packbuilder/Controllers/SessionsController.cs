using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Packbuilder.Interfaces;
using Packbuilder.Exceptions;
using Packbuilder.Dto.Create;

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
            catch (ExceptionBase)
            {
                return Unauthorized();
            }
        }
    }
    
}