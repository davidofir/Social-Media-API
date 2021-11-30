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
using API.Models.Responses;
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
        // POST /api/users 
        [HttpPost]
        public async Task<IActionResult> AddUsers([FromBody] User user)
        {
            var emailExists = await _context.Users.FirstOrDefaultAsync(usr => usr.Email.ToLower().Equals(user.Email.ToLower()));
            if (emailExists != null)
            {
                return BadRequest(new ErrorDTO
                {
                    Status = "400",
                    Title = "Email Already Exists",
                    Details = "The Email already exists, would you like to sign in instead?"
                });
            }
            if (!ModelState.IsValid)
            {
                var errorsList = (from item in ModelState where item.Value.Errors.Any() select item.Value.Errors[0]).ToList();
                return BadRequest(new ErrorDTO
                {
                    Status = "400",
                    Title = "Invalid format",
                    Details = errorsList[0].ErrorMessage
                });
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        // POST /api/users/{id}/image
        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddImageToUser(string id, [FromBody] Image img)
        {
            var user = _context.Users.Include(x => x.Images).ThenInclude(x => x.Tags).SingleOrDefault(x => x.Id.Equals(new Guid(id)));

            var tagsList = ImageHelper.GetTags(img.Url);

            img.User = user;
            img.PostingDate = DateTime.Now;
            img.Tags = new List<Tag>();
            foreach (var tg in tagsList)
            {

                var existingTag = _context.Tags.FirstOrDefault(x => x.Text.ToLower().Equals(tg.ToLower()));
                if (existingTag == null)
                {


                    img.Tags.Add(new Tag
                    {
                        Text = tg
                    });
                }
                else
                {
                    img.Tags.Add(existingTag);
                }
            }
            user.Images.Add(img);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // GET /api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAndLast10Images(string id)
        {
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user == null)
                return BadRequest();

            var imagesList = new List<string>();
            var images = _context.Images.Include(x => x.User).Where(x => x.User.Id.Equals(new Guid(id))).OrderByDescending(x => x.PostingDate);
            var total = await images.CountAsync();
            if (total >= 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    imagesList.Add(images.ElementAt(i).Url);
                }
                return Ok(new UserDTO
                {
                    Email = user.Email,
                    imageUrls = imagesList,
                    Name = user.Name
                });
            }
            foreach (var image in images)
            {
                imagesList.Add(image.Url);
            }
            return Ok(new UserDTO
            {
                Email = user.Email,
                imageUrls = imagesList,
                Name = user.Name
            });
        }
        // GET api/users/{id}/images
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetUserImages(string id, [FromQuery] int pagenumber)
        {
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user == null)
            {
                return BadRequest();
            }
            var imagesList = new List<ImageDTO>();
            var result = _context.Images.Include(x => x.User).Where(x => x.User.Id.Equals(new Guid(id))).OrderBy(x => x.PostingDate);
            var total = await result.CountAsync();
            foreach (var image in result)
            {
                imagesList.Add(new ImageDTO
                {
                    Id = image.Id,
                    UserName = user.Name,
                    Url = image.Url
                });
            }
            var response = ResponseHelper<ImageDTO>.GetPagedResponse("/api/images", imagesList, pagenumber, 10, total);
            return Ok(response);
        }
        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user == null)
                return BadRequest();
            var foundImages = _context.Images.Include(x => x.User).Where(x => x.User.Id.Equals(new Guid(id)));
            _context.Images.RemoveRange(foundImages);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}