using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models.Persistence;
using API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly DataContext _context;
        public ImagesController(DataContext context)
        {
            _context = context;
        }
        // GET /api/images v
        [HttpGet]
        public async Task<IActionResult> GetAllImages([FromQuery] int pageNumber)
        {
            var result = _context.Images.Include(x => x.User).OrderBy(x => x.PostingDate).Skip((pageNumber - 1) * 10).Take(10);
            var imgList = new List<ImageDTO>();
            foreach (var res in result)
            {
                imgList.Add(new ImageDTO
                {
                    Id = res.Id,
                    Url = res.Url,
                    UserName = res.User.Name
                });
            }
            var total = await _context.Images.CountAsync();
            var response = ResponseHelper<ImageDTO>.GetPagedResponse("/api/images", imgList, pageNumber, 10, total);
            return Ok(response);
        }
        // GET /api/images/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(string id)
        {
            var image = await _context.Images.Include(x => x.Tags).Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id.Equals(new Guid(id)));
            if (image == null)
            {
                return NotFound();
            }
            var tagsList = new List<string>();
            foreach (var tag in image.Tags)
            {
                tagsList.Add(tag.Text);
            }
            return Ok(new ExpandedImageDTO
            {
                Id = image.Id,
                Tags = tagsList,
                Url = image.Url,
                UserId = image.User.Id
            });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImages()
        {
            if (_context.Tags.Any())
                _context.Tags.Remove(_context.Tags.First());
            if (_context.Images.Any())
                _context.Images.Remove(_context.Images.First());
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}