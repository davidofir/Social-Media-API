using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using API.Models.Entities;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
//using Microsoft.EntityFrameworkCore;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(string id)
        {
            var image = await _context.Images.FindAsync(new Guid(id));
            if (image == null)
            {
                return NotFound();
            }
            return Ok(image);
        }
        [HttpGet]
        public IActionResult GetImageByTagName([FromQuery] string tag)
        {
            var data = _context.Tags
                .Include(x => x.Images)
                .Where(tg => tg.Text.ToLower().Equals(tag.ToLower()));
            if (data == null)
                return NotFound();
            return Ok(data);
        }
    }
}