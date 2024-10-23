using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Party.Models;

namespace Party.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<ulong>, ulong>
    { 
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {      
        
        }
        public DbSet<UserAccount> UserAccount { get; set; }


    }
}
