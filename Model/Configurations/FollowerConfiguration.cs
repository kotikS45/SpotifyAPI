using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Model.Configurations;

public class FollowerConfiguration : IEntityTypeConfiguration<Follower>
{
    public void Configure(EntityTypeBuilder<Follower> builder)
    {
        builder.ToTable("Follower");

        builder.HasKey(f => new
        {
            f.UserId,
            f.ArtistId
        });
    }
}