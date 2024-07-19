using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class MigrationService(
    DataContext context
    ) : IMigrationService
{

    public async Task MigrateLatestAsync()
    {
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        await context.Database.MigrateAsync();
    }

    private async Task<IEnumerable<string>> GetPendingMigrationsAsync()
        => await context.Database.GetPendingMigrationsAsync();

    private async Task<bool> IsPendingMigrationBeforeOrEqualsAsync(string name)
    {
        var migrations = await context.Database.GetPendingMigrationsAsync();

        return migrations.TakeWhile(pm => pm != name)
            .Contains(name);
    }

    private async Task<bool> IsPendingMigrationAfterOrEqualsAsync(string name)
    {
        var migrations = await context.Database.GetPendingMigrationsAsync();

        return migrations.SkipWhile(pm => pm != name)
            .Contains(name);
    }

    private async Task<bool> IsMigrationPendingAsync(string name)
    {
        var migrations = await context.Database.GetPendingMigrationsAsync();

        return migrations.Contains(name);
    }

    private async Task<bool> IsMigrationNotPendingAsync(string name)
        => !await IsMigrationPendingAsync(name);
}