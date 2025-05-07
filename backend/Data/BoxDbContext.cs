using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class BoxDbContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }
    public BoxDbContext(DbContextOptions options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Box>().HasKey(b => b.Code);
    }
}