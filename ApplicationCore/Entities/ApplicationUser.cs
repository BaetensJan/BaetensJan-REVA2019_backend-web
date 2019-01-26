using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public School School { get; set; }
    }
}