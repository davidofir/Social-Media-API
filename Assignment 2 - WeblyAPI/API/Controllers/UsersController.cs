using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        [HttpGet("/{id}")]
        //api/users/{id}
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }
    }
}