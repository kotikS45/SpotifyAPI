using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Mapper;
using SpotifyAPI.Services;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    configuration => configuration
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);

app.UseAuthorization();

app.MapControllers();

app.Run();