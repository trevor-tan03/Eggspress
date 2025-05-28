using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class BoxDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }
    public DbSet<Models.File> Files { get; set; }
    public BoxDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Box>().HasKey(b => b.Code);

        modelBuilder.Entity<Models.File>()
            .HasOne(f => f.Box)
            .WithMany(b => b.Files)
            .HasForeignKey(f => f.BoxCode)
            .HasPrincipalKey(b => b.Code)
            .OnDelete(DeleteBehavior.Cascade);
    }
}