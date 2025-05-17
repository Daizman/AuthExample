using AuthExample.Configurations;
using AuthExample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthExample.Database;

public class AppDbContext(
    IOptions<DatabaseOptions> options,
    IWebHostEnvironment environment,
    ILogger<AppDbContext> logger
) : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<JwtToken> JwtTokens { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql(options.Value.ConnectionString);

        if (environment.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(message => logger.LogInformation(message), LogLevel.Information);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
    }
}
