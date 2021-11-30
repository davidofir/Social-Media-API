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
        // GET /api/images
        [HttpGet]
        public async Task<IActionResult> GetAllImages([FromQuery] int pageNumber)
        {
            var result = _context.Images.Include(x => x.User).OrderBy(x => x.PostingDate);
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
            // Check if the ID format is valid
            Guid x;
            if (!Guid.TryParse(id, out x))
            {
                return BadRequest(new ErrorDTO
                {
                    Status = "400",
                    Title = "Invalid Format",
                    Details = "The specified format of the given user ID is invalid"
                });
            }
            // Find only the images that belong to the given user's id
            var image = await _context.Images.Include(x => x.Tags).Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id.Equals(new Guid(id)));
            if (image == null)
            {
                return BadRequest(new ErrorDTO
                {
                    Status = "404",
                    Title = "User Not Found",
                    Details = "The user could not be found in our database"
                });
            }
            // convert the list of tag objects to a list of strings of tags
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
        // GET /api/images/byTag?tag=cars
        [HttpGet("byTag")]
        public async Task<IActionResult> GetImagesByTagName(string tag, int pagenumber)
        {
            // get the errors that were when submitting the user form
            var tags = _context.Tags.Include(x => x.Images).ThenInclude(x => x.User).Where(x => x.Text.ToLower().Equals(tag.ToLower()));
            if (tags == null)
                return BadRequest(new ErrorDTO
                {
                    Status = "404",
                    Title = "Tag Not Found",
                    Details = "The tag that you selected could not be found"
                });
            // find the total
            var total = await tags.CountAsync();
            var imgList = new List<ImageDTO>();
            // take the images from the list of tags that correspond to the queried tag
            foreach (var tg in tags)
            {
                foreach (var img in tg.Images)
                {
                    imgList.Add(new ImageDTO
                    {
                        Id = img.Id,
                        Url = img.Url,
                        UserName = img.User.Name
                    });
                }
            }
            // paginate the responses
            var response = ResponseHelper<ImageDTO>.GetPagedResponse("/api/images", imgList, pagenumber, 10, total);
            return Ok(response);
        }
        // GET /api/images/populartags
        [HttpGet("populartags")]
        public IActionResult GetPopularTags()
        {
            var mostPopularDTO = new List<TagDTO>();
            var mostPopular = new List<Tag>();
            int min = 0;
            foreach (var tg in _context.Tags.Include(x => x.Images))
            {
                // add 5 tags initially
                if (mostPopular.Count < 5)
                {
                    mostPopular.Add(tg);
                }
                else
                {
                    // sort the list by the popularity
                    mostPopular = mostPopular.OrderByDescending(x => x.Images.Count).ToList<Tag>();
                    // take the smallest element (the first one)
                    min = mostPopular[mostPopular.Count - 1].Images.Count;
                    // if the current image's popularity is larger than the least popular image in the list
                    //, replace the least popular image with the new image
                    if (tg.Images.Count > min)
                    {
                        mostPopular[mostPopular.Count - 1] = tg;
                        mostPopular = mostPopular.OrderByDescending(x => x.Images.Count).ToList<Tag>();
                        min = mostPopular[mostPopular.Count - 1].Images.Count;
                    }
                }
            }
            // convert the tags list of objects to a list of TagDTOs to eliminate memory waste 
            // (allows us to avoid creating too many objects that will not be used)
            for (int i = 0; i < mostPopular.Count; i++)
            {
                mostPopularDTO.Add(new TagDTO
                {
                    Count = mostPopular[i].Images.Count,
                    Tag = mostPopular[i].Text
                });
            }
            return Ok(mostPopularDTO);
        }
    }
}