using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(p => p.Platform)
            .HasForeignKey(p => p.PlatformId);

        // modelBuilder.Entity<Command>()
        //     .HasOne<Platform>()
        //     .WithMany(c => c.Commands)
        //     .HasForeignKey(c => c.PlatformId);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Platform> Platforms { get; set; } = default!;
    public DbSet<Command> Commands { get; set; } = default!;
}
