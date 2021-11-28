using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Models.Entities;
using API.Models.Helpers;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        //api/users/{id}
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(new Guid(id));

            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddUsers([FromBody] User user)
        {
            var emailExists = await _context.Users.FirstOrDefaultAsync(usr => usr.Email.ToLower().Equals(user.Email.ToLower()));
            if (!ModelState.IsValid || emailExists != null)
            {
                return BadRequest();
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddImageToUser(string id, [FromBody] Image img)
        {
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user == null)
            {
                return BadRequest();
            }
            var tagsList = ImageHelper.GetTags(img.Url);
            img.User = user;
            img.Tags = new List<Tag>();
            foreach (var tg in tagsList)
            {
                img.Tags.Add(new Tag
                {
                    Text = tg
                });
            }
            _context.Images.Add(img);
            await _context.SaveChangesAsync();
            return Ok(img);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user == null)
                return BadRequest();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}