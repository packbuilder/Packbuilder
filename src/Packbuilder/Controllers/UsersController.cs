using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController(IPasswordHasher<User> passwordHasher, PackbuilderContext context) : ControllerBase
    {
        [HttpGet("{id:int}")]
        [EndpointName("GetUsers")]
        public async Task<ActionResult<User>> GetUser([FromRoute] int id)
        {
            return Ok(await context.Users.SingleOrDefaultAsync(u => u.Id == id));
        }

        [HttpPost]
        [EndpointName("CreateAccount")]
        public async Task<ActionResult<User>> PostUser([FromBody] CreateUserDto body)
        {
            User user = new User()
            {
                Name = body.Name,
                Email = body.Email,
                Avatar = "Use generic avatar link or something"
            };

            user.SetPassword(passwordHasher, body.Password);

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }
    }
}

