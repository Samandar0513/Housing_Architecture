using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Housing_Architecture.DataAccess.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
    public DbSet<PropertyPhoto> PropertyPhotos { get; set; }
    public DbSet<PropertyDocument> PropertyDocuments { get; set; }
    public DbSet<UserOTPs> UserOTPs { get; set; }
    public DbSet<TempUser> TempUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // ? SEED DATA
        modelBuilder.SeedData();
    }
}
