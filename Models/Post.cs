using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Blog_API.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public IdentityUser User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
