using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController(IPasswordHasher<User> passwordHasher, PackbuilderContext context, ISessionService sessionService) : ControllerBase
    {
        [HttpGet()]
        [EndpointName("GetUsers")]
        public async Task<ActionResult<User[]>> GetUsers([FromRoute] int id)
        {
            return Ok(await context.Users.ToArrayAsync());
        }

        [HttpGet("{id:int}")]
        [EndpointName("GetUser")]
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

        [HttpPut("{username}")]
        [EndpointName("UpdateUser")]
        [Authorize]
        public async Task<ActionResult<User>> UpdateUser([FromBody] UpdateUserDto body, [FromRoute] string username)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            User? user = await context.Users.SingleOrDefaultAsync(u => u.Name == username);

            if (user is null)
            {
                return NotFound();
            }

            if (currentUser is null)
            {
                return NotFound();
            }

            if (currentUser.Id != user.Id)
            {
                return Unauthorized();
            }

            if (body.Email is not null)
            {
                user.Email = body.Email;
            }
            if (body.Name is not null)
            {
                user.Name = body.Name;
            }
            if (body.Password is not null)
            {
                user.SetPassword(passwordHasher, body.Password);
            }

            context.Users.Update(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }

        [Authorize]
        [HttpDelete("{username}")]
         public async Task<ActionResult<User>> DeleteUser([FromRoute] string username)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            User? user = await context.Users.SingleOrDefaultAsync(u => u.Name == username);

            if (user is null)
            {
                return NotFound();
            }

            if (currentUser?.Id != user.Id)
            {
                return Unauthorized();
            }

            context.Users.RemoveRange(user);
            await context.SaveChangesAsync();
            return Ok(user);
        }
    }
}

