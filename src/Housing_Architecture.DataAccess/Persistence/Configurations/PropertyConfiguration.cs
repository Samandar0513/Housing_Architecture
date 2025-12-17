using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Housing_Architecture.DataAccess.Persistence.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Description)
            .HasMaxLength(2000);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Property(p => p.TotalArea)
            .HasPrecision(10, 2);

        builder.Property(p => p.ContactName)
            .HasMaxLength(100);

        builder.Property(p => p.ContactPhone)
            .HasMaxLength(20);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Properties)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.District)
            .WithMany(d => d.Properties)
            .HasForeignKey(p => p.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Region)
            .WithMany(r => r.Properties)
            .HasForeignKey(p => p.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
