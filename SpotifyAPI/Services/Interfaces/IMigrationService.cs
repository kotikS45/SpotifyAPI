namespace SpotifyAPI.Services.Interfaces;

public interface IMigrationService
{
    Task MigrateLatestAsync();
}