using Microsoft.AspNetCore.Identity;

namespace Red_mango_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
