using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionsController : ControllerBase
    {
        private readonly BlogContext _context;

        public ReactionsController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/Reactions
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Reaction>>> GetReactions()
        {
            //return await _context.Reactions.ToListAsync();
            var reactions = await _context.Reactions.Include(r => r.Post) // Include the Post navigation property
        .Include(r => r.User)
        .ToListAsync();

    if (reactions == null || !reactions.Any())
    {
        return NotFound("Reactions not found for the specified post.");
    }

    return Ok(reactions);
        }

        // GET: api/Reactions/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Reaction>> GetReaction(int id)
        {
            /*var reaction = await _context.Reactions.FindAsync(id);

            if (reaction == null)
            {
                return NotFound();
            }

            return reaction;*/
            var reactions = await _context.Reactions.Include(r => r.Post) // Include the Post navigation property
        .Include(r => r.User)
        .Where(r => r.ReactionId == id)
        .ToListAsync();

    if (reactions == null || !reactions.Any())
    {
        return NotFound("Reactions not found for the specified post.");
    }

    return Ok(reactions);
        }

        // PUT: api/Reactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> PutReaction(int id, Reaction reaction)
        {
            if (id != reaction.ReactionId)
            {
                return BadRequest();
            }

            _context.Entry(reaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReactionExists(id))
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

        // POST: api/Reactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Reaction>> PostReaction(Reaction reaction)
        {
            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReaction", new { id = reaction.ReactionId }, reaction);
        }

        // DELETE: api/Reactions/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> DeleteReaction(int id)
        {
            var reaction = await _context.Reactions.FindAsync(id);
            if (reaction == null)
            {
                return NotFound();
            }

            _context.Reactions.Remove(reaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReactionExists(int id)
        {
            return _context.Reactions.Any(e => e.ReactionId == id);
        }
    }
}
