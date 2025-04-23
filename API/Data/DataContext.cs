using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options): DbContext(options)
{
    public DbSet<Partner> Partners {get; set;}
    public DbSet<Item> Items {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Partner>()
            .HasMany(p => p.Items)
            .WithOne(p => p.Partner)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Partner>()
            .HasIndex(p => p.PartnerRefNo)
            .IsUnique();

        modelBuilder.Entity<Item>()
            .HasIndex(i => i.PartnerItemRef)
            .IsUnique();

    }
}
