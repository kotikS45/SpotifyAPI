using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Model.Configurations;

public class TrackGenreConfiguration : IEntityTypeConfiguration<TrackGenre>
{
    public void Configure(EntityTypeBuilder<TrackGenre> builder)
    {
        builder.ToTable("TrackGenre");

        builder.HasKey(pt => new
        {
            pt.TrackId,
            pt.GenreId
        });
    }
}