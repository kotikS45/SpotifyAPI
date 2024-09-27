using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpotifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_Albums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Image = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ArtistId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Albums");
        }
    }
}
/*
using AutoMapper;
using Model.Entities;
using Model.Entities.Identity;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<RegisterVm, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)); // Можна налаштувати додаткові правила маппінгу

            CreateMap<User, UserVm>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateArtistMaps();
            CreateAlbumMaps();
            CreateTrackMaps();
            CreatePlaylistMaps();
            CreateGenreMaps();
        }

        private void CreateArtistMaps()
        {
            CreateMap<Artist, ArtistVm>();
            CreateMap<ArtistCreateVm, Artist>()
                .ForMember(c => c.Image, opt => opt.Ignore()); // Можливо, слід додати обробку для Image пізніше
        }

        private void CreateAlbumMaps()
        {
            CreateMap<Album, AlbumVm>();
            CreateMap<AlbumCreateVm, Album>()
                .ForMember(c => c.Image, opt => opt.Ignore()); // Коментар: поле Image може бути встановлено через інший механізм
        }

        private void CreateTrackMaps()
        {
            CreateMap<Track, TrackVm>();
            CreateMap<TrackCreateVm, Track>()
                .ForMember(c => c.Path, opt => opt.Ignore()) // Якщо Path приходить з іншого джерела
                .ForMember(c => c.Duration, opt => opt.Ignore()) // Можливо, Duration рахується автоматично
                .ForMember(c => c.Genres, opt => opt.Ignore()); // Genres може бути оброблено окремо
        }

        private void CreatePlaylistMaps()
        {
            CreateMap<Playlist, PlaylistVm>();
            CreateMap<PlaylistCreateVm, Playlist>()
                .ForMember(c => c.Image, opt => opt.Ignore()); // Можливо, слід додати обробку для Image пізніше
        }

        private void CreateGenreMaps()
        {
            CreateMap<Genre, GenreVm>();
            CreateMap<GenreCreateVm, Genre>();

            CreateMap<TrackGenre, GenreVm>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Genre.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Genre.Name));
        }
    }
}

*/
