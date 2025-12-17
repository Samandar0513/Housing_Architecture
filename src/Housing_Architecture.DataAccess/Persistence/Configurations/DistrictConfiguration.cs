using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Housing_Architecture.DataAccess.Persistence.Configurations;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(d => d.Region)
            .WithMany(r => r.Districts)
            .HasForeignKey(d => d.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
