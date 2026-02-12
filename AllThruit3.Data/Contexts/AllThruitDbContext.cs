using AllThruit3.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AllThruit3.Data.Contexts;

public class AllThruitDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Media> Media { get; set; }
    public DbSet<BlobMedia> BlobMedia { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public AllThruitDbContext(DbContextOptions<AllThruitDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Identity Guid keys

        // ApplicationUser extras
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.ExtraField1).HasMaxLength(50);
            b.Property(u => u.ExtraField2).HasMaxLength(50);
            b.Property(u => u.ExtraField3).HasMaxLength(50);
        });

        modelBuilder.Entity<Review>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Media>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BlobMedia>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(r => r.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);
    }
}