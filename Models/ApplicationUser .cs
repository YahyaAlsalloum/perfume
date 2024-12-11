using Microsoft.AspNetCore.Identity;

namespace perfume.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        // Relationships
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
