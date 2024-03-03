using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Blog_API.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
        public Post Post { get; set; }
        public IdentityUser User { get; set; }
    }
}
