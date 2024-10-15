using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Entities;

namespace Model.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.ToTable("Tracks");

        builder.Property(a => a.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Duration)
            .IsRequired();

        builder.Property(a => a.Path)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.Image)
            .HasMaxLength(255)
            .IsRequired();
    }
}