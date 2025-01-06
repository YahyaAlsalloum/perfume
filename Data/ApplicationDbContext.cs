using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using perfume.Models;
using System.Reflection.Emit;

namespace perfume.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<perfume.Models.Product> Product { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder Builder)
        {
            base.OnModelCreating(Builder);
            Builder.Entity<ApplicationUser>().ToTable("AspNetUsers");

        }

        public DbSet<perfume.Models.Order> Order { get; set; } = default!;
        public DbSet<OrderProduct> OrderProducts { get; set; } 
        public DbSet<Product> Products { get; set; }
        public DbSet<perfume.Models.BasePerfume> BasePerfume { get; set; } = default!;
        public DbSet<perfume.Models.Message> Message { get; set; } = default!;
        public DbSet<perfume.Models.Favorite> Favorite { get; set; } = default!;
    }

}