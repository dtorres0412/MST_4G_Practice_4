using Microsoft.EntityFrameworkCore;
using MST_4G_Self_Practice_4.Models;

namespace MST_4G_Self_Practice_4.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Zo> Zo { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Zo>(entity =>
        {
            entity.ToTable("zo");
            entity.HasKey(c => c.ZoId);
            entity.Property(c => c.ZoId).HasColumnName("zo_id");
            entity.Property(c => c.ZoNo).HasColumnName("zo_no");
            entity.Property(c => c.ZoName).HasColumnName("zo_name");
        });
    } 
}