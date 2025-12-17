using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Housing_Architecture.DataAccess.Persistence.Configurations;

public class PropertyPhotoConfiguration : IEntityTypeConfiguration<PropertyPhoto>
{
    public void Configure(EntityTypeBuilder<PropertyPhoto> builder)
    {
        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(pp => pp.Property)
            .WithMany(p => p.Photos)
            .HasForeignKey(pp => pp.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
