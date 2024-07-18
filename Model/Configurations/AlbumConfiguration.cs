using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Entities;

namespace Model.Configurations;

public class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        builder.ToTable("Albums");

        builder.Property(a => a.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.ReleaseDate)
            .HasConversion(
				v => v.ToUniversalTime(),
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            .IsRequired();


        builder.Property(a => a.Image)
            .HasMaxLength(255)
            .IsRequired();
    }
}