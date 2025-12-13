using AuthServer.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Infrastructure.Context
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid> 
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.UseOpenIddict();

            builder.HasDefaultSchema("auth");
        }
    }
}
