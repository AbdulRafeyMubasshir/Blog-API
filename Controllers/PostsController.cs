using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Blog_API.DTO;



namespace Blog_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BlogContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly ILogger<PostsController> _logger;

        public PostsController(BlogContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostResponseDTO>>> GetPosts()
        {
            
            return await _context.Posts
                .Include(p => p.User)
                .Select(p => new PostResponseDTO
                {
                    
                    postId = p.PostId,
                    title = p.Title,
                    content = p.Content,
                    
                    author = p.User.UserName
                })
                .ToListAsync();
                //_logger.LogInformation($"Posts retrieved successfully");


        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponseDTO>> GetPost(int id)
        {
            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            // Project the result into a DTO
            var postDto = new PostResponseDTO
            {
                postId = post.PostId,
                title = post.Title,
                content = post.Content,
                author = post.User.UserName
            };

            return Ok(postDto);
        }

        // PUT: api/Posts/5
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> PutPost(int id, PostRequestDTO post)
        {
            if (id != post.postId)
            {
                return BadRequest();
            }


            var postToUpdate = await _context.Posts.FindAsync(id);

            if (postToUpdate == null)
            {
                return NotFound();
            }

            // Map properties from DTO to entity
            postToUpdate.Title = post.title;
            postToUpdate.Content = post.content;

            


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts

        [HttpPost]
        [Authorize(Roles = "Admin,Author")]
        public async Task<ActionResult<PostResponseDTO>> PostPost(PostRequestDTO post)
        {
            // Get the user ID from the authentication token
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the token
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            // Access email from the decoded token
            var email = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            var user = await _userManager.FindByEmailAsync(email);


            // Create a new post with the provided data and associate it with the user
            var newpost = new Post
            {
                PostId = post.postId,
                User = null, 
                Title = post.title,
                Content = post.content
            };
            _context.Posts.Attach(newpost);
            newpost.User = new IdentityUser { Id = user.Id }; // Create a new instance with the given user ID

            _context.Posts.Add(newpost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = newpost.PostId }, new PostResponseDTO
                                                                                {
                                                                                postId = newpost.PostId,
                                                                                title = newpost.Title,
                                                                                content = newpost.Content,
                                                                                author = newpost.User.UserName
                                                                                });
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Author")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{postId}/comments")]
        [Authorize] 
        public async Task<IActionResult> AddComment(int postId, CommentDTO comment)
        {
            // Retrieve the post
            var post = await _context.Posts.FindAsync(postId);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the token
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            // Access email from the decoded token
            var email = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Set the PostId and UserId for the comment
            var newcomment = new Comment {
                CommentId = comment.CommentId,
                Text = comment.Content,
                PostId = postId,
                Post = post
            };
            
            _context.Comments.Attach(newcomment);
            newcomment.User = new IdentityUser { Id = user.Id }; // Create a new instance with the given user ID

            _context.Comments.Add(newcomment);
            await _context.SaveChangesAsync();

            return Ok("Comment added successfully.");
        }

        [HttpGet("/api/posts/{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsForPost(int postId)
        {
            var comments = await _context.Comments.Include(c => c.Post) // Include the Post navigation property
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound("Comments not found for the specified post.");
            }

            return Ok(comments);
        }

        [HttpPost("{postId}/reactions")]
        [Authorize] 
        public async Task<IActionResult> AddReaction(int postId, ReactionDTO reaction)
        {
            // Retrieve the post
            var post = await _context.Posts.FindAsync(postId);
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Decode the token
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            // Access email from the decoded token
            var email = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;
            var user = await _userManager.FindByEmailAsync(email);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Set the PostId and UserId for the comment
            var newreaction = new Reaction {
                ReactionId = reaction.ReactionId,
                ReactionType = reaction.ReactionType,
                PostId = postId,
                Post = post
            };
            
            _context.Reactions.Attach(newreaction);
            newreaction.User = new IdentityUser { Id = user.Id }; // Create a new instance with the given user ID

            _context.Reactions.Add(newreaction);
            await _context.SaveChangesAsync();

            return Ok("Reaction added successfully.");
        }


        [HttpGet("/api/posts/{postId}/reactions")]
        public async Task<ActionResult<IEnumerable<Reaction>>> GetReactionsForPost(int postId)
        {
            var reactions = await _context.Reactions.Include(r => r.Post) // Include the Post navigation property
                .Include(r => r.User)
                .Where(r => r.PostId == postId)
                .ToListAsync();

            if (reactions == null || !reactions.Any())
            {
                return NotFound("Reactions not found for the specified post.");
            }

            return Ok(reactions);
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
