using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Model.Context;
using Model.Entities.Identity;
using SpotifyAPI.Mapper;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Follower;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.PlaylistTrack;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services;
using SpotifyAPI.Seeder;
using SpotifyAPI.Seeder.Interfaces;
using SpotifyAPI.Validators.Artist;
using System.Text;
using SpotifyAPI.Services.Interfaces;
using SpotifyAPI.Services.Pagination;
using SpotifyAPI.SMTP;
using SpotifyAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assemblyName = AssemblyService.GetAssemblyName();

builder.Services.AddDbContext<DataContext>(
    options => {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("Npgsql"),
            npgsqlOptions => npgsqlOptions.MigrationsAssembly(assemblyName)
        );
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    }
);

builder.Services
    .AddIdentity<User, Role>(options => {
        options.Stores.MaxLengthForKeys = 128;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

var singinKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(
        builder.Configuration["Authentication:Jwt:SecretKey"]
            ?? throw new NullReferenceException("Authentication:Jwt:SecretKey")
    )
);

builder.Services
    .AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options => {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = singinKey,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1048576000;
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description = "Jwt Auth header using the Bearer scheme",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        }
    );
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddAutoMapper(typeof(AppMapProfile));
builder.Services.AddValidatorsFromAssemblyContaining<PlaylistCreateValidator>();

builder.Services.AddScoped<IMigrationService, MigrationService>();

builder.Services.AddScoped<IIdentitySeeder, IdentitySeeder>();
builder.Services.AddScoped<IDataSeeder, DataSeeder>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IImageValidator, ImageValidator>();

builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddScoped<IScopedIdentityService, ScopedIdentityService>();

builder.Services.AddTransient<IAudioService, AudioService>();
builder.Services.AddTransient<IAudioValidator, AudioValidator>();

builder.Services.AddTransient<IExistingEntityCheckerService, ExistingEntityCheckerService>();

builder.Services.AddTransient<IAccountsControllerService, AccountsControllerService>();

builder.Services.AddTransient<IArtistsControllerService, ArtistsControllerService>();
builder.Services.AddTransient<IPaginationService<ArtistVm, ArtistFilterVm>, ArtistPaginationService>();

builder.Services.AddTransient<IAlbumsCotrollerService, AlbumsControllerService>();
builder.Services.AddTransient<IPaginationService<AlbumVm, AlbumFilterVm>, AlbumPaginationService>();

builder.Services.AddTransient<ITrackControllerService, TracksControllerService>();
builder.Services.AddTransient<IPaginationService<TrackVm, TrackFilterVm>, TrackPaginationService>();

builder.Services.AddTransient<IPlaylistControllerService, PlaylistsControllerService>();
builder.Services.AddTransient<IPaginationService<PlaylistVm, PlaylistFilterVm>, PlaylistPaginationService>();

builder.Services.AddTransient<IPlaylistTrackControllerService, PlaylistTrackControllerService>();
builder.Services.AddTransient<IPaginationService<TrackVm, PlaylistTrackFilterVm>, PlaylistTracksPaginationService>();

builder.Services.AddTransient<IFollowerControllerService, FollowerControllerService>();
builder.Services.AddTransient<IPaginationService<ArtistVm, FollowerFilterVm>, FollowerPaginationService>();

builder.Services.AddTransient<ILikeControllerService, LikeControllerService>();
builder.Services.AddTransient<IPaginationService<TrackVm, LikeFilterVm>, LikePaginationService>();

builder.Services.AddTransient<IGenreControllerService, GenreControllerService>();

builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.Configure<ApiKeys>(builder.Configuration.GetSection("ApiKeys"));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// Images
string imagesDirPath = app.Services.GetRequiredService<IImageService>().ImagesDir;

if (!Directory.Exists(imagesDirPath))
{
    Directory.CreateDirectory(imagesDirPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesDirPath),
    RequestPath = "/images"
});

// Audio
string audioDirPath = app.Services.GetRequiredService<IAudioService>().AudioDir;

if (!Directory.Exists(audioDirPath))
{
    Directory.CreateDirectory(audioDirPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(audioDirPath),
    RequestPath = "/audio"
});

// Cors
app.UseCors(
    configuration => configuration
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);

app.UseAuthorization();

app.MapControllers();

await using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateLatestAsync();
    await scope.ServiceProvider.GetRequiredService<IIdentitySeeder>().SeedAsync();
    await scope.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync();
}

app.Run();