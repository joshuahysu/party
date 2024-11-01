using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Party.Models;
using Party.Services.Multilayer;

namespace Party.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<ulong>, ulong>
    { 
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {      
        
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Closure>()
                 .HasKey(c => new { c.Ancestor, c.Descendant });
            modelBuilder.Entity<Node>()
                  .HasOne(n => n.Parent)
                  .WithMany(n => n.Children)
                  .HasForeignKey(n => n.ParentId);
        }
        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<Closure> Closures { get; set; }
        public DbSet<Node> Nodes { get; set; }
    }
}
