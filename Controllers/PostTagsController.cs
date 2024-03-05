using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_API.Models;
using Microsoft.AspNetCore.Authorization;
using Blog_API.DTO;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostTagsController : ControllerBase
    {
        private readonly BlogContext _context;

        public PostTagsController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/PostTags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostTags>>> GetPostTags()
        {
            return await _context.PostTags.ToListAsync();
        }

        // GET: api/PostTags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostTags>> GetPostTags(int id)
        {
            var postTags = await _context.PostTags.FindAsync(id);

            if (postTags == null)
            {
                return NotFound();
            }

            return postTags;
        }

        // PUT: api/PostTags/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> PutPostTags(int id, PostTags postTags)
        {
            if (id != postTags.Id)
            {
                return BadRequest();
            }

            _context.Entry(postTags).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostTagsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PostTags
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin,Author")]
        public async Task<ActionResult<PostTags>> PostPostTags(PostTagDTO PostTagDTO)
        {/////////////////////////////////eeeeeeeee
            var post = await _context.Posts.FindAsync(PostTagDTO.PostId);
            var tag = await _context.Tags.FindAsync(PostTagDTO.TagId);
            var postTags = new PostTags{
                Id = PostTagDTO.Id,
                PostId = PostTagDTO.PostId,
                TagId = PostTagDTO.TagId,
                Post = post,
                Tag = tag
            };
            _context.PostTags.Add(postTags);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostTags", new { id = postTags.Id }, postTags);
        }

        // DELETE: api/PostTags/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> DeletePostTags(int id)
        {
            var postTags = await _context.PostTags.FindAsync(id);
            if (postTags == null)
            {
                return NotFound();
            }

            _context.PostTags.Remove(postTags);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostTagsExists(int id)
        {
            return _context.PostTags.Any(e => e.Id == id);
        }
    }
}
