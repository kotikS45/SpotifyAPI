using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Model.Context;
using SpotifyAPI.Mapper;
using SpotifyAPI.Services;
using SpotifyAPI.Services.Interfaces;
using SpotifyAPI.Validators.Artist;
using System.Data;

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

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AppMapProfile));
builder.Services.AddValidatorsFromAssemblyContaining<ArtistCreateValidator>();

builder.Services.AddScoped<IMigrationService, MigrationService>();

builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IImageValidator, ImageValidator>();

builder.Services.AddTransient<IAudioService, AudioService>();
builder.Services.AddTransient<IAudioValidator, AudioValidator>();

builder.Services.AddTransient<IExistingEntityCheckerService, ExistingEntityCheckerService>();

builder.Services.AddTransient<IArtistsControllerService, ArtistsControllerService>();
builder.Services.AddTransient<IAlbumsCotrollerService, AlbumsControllerService>();
builder.Services.AddTransient<ITrackControllerService, TracksControllerService>();


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
}

app.Run();