using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model.Entities;

namespace Model.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("Genres");

        builder.Property(g => g.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(g => g.Name)
            .IsUnique();

        builder.Property(a => a.Image)
            .HasMaxLength(255)
            .IsRequired();
    }
}