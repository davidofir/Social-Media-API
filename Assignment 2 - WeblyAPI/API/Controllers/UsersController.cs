using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models.Helpers;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
            var user = await _context.Users.Include(x => x.Images).ThenInclude(x => x.Tags).SingleOrDefaultAsync(x => x.Id.Equals(new Guid(id)));
            var imgAddressList = new List<string>();
            foreach (var img in user.Images)
            {
                imgAddressList.Add(img.Url);
            }
            if (user != null)
                return Ok(new UserDTO
                {
                    Email = user.Email,
                    imageUrls = imgAddressList,
                    Name = user.Name
                });
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
            var user = await _context.Users.Include(x => x.Images).ThenInclude(x => x.Tags).SingleOrDefaultAsync(x => x.Id.Equals(new Guid(id)));
            if (user == null)
            {
                return BadRequest();
            }
            var tagsList = ImageHelper.GetTags(img.Url);

            img.PostingDate = DateTime.Now;
            img.Tags = new List<Tag>();
            if (tagsList.Any())
            {
                foreach (var tag in tagsList)
                {
                    var existingTag = img.Tags.FirstOrDefault(x => x.Text.ToLower().Equals(tag.ToLower()));
                    if (existingTag == null)
                    {
                        img.Tags.Add(new Tag
                        {
                            Text = tag
                        });
                    }
                    else
                    {
                        existingTag.Images.Add(img);
                    }
                }
                img.User = user;
                var postedImage = await _context.Images.AddAsync(img);
                user.Images.Add(img);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var convertedId = new Guid(id);
            var user = await _context.Users.FindAsync(convertedId);
            if (user == null)
                return BadRequest();
            await _context.Images.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Id.Equals(new Guid(id)));
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}