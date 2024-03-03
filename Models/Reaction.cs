using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Blog_API.Models
{
    public class Reaction
    {
        public int ReactionId { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public IdentityUser User { get; set; }
        public String ReactionType { get; set; }
    }
}
