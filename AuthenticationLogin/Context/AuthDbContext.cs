using AuthenticationLogin.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationLogin.Context;

public class AuthDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public AuthDbContext(DbContextOptions<AuthDbContext> options) 
        : base(options)
    {
        Database.Migrate();
    }
}
