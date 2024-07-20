namespace SeatsInventory.Seeder;

public class SeatSeederHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<SeatSeeder>();
        await seeder.SeedSeatsAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}