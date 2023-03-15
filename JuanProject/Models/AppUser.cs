using Microsoft.AspNetCore.Identity;

namespace JuanProject.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
