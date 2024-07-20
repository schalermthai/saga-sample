using SeatsInventory.Domain;

namespace SeatsInventory.Seeder;

public class SeatSeeder(ISeatRepository seatRepository)
{
    public async Task SeedSeatsAsync()
    {
        var rows = new[] { 'A', 'B', 'C', 'D', 'E', 'F' };
        const int seatNumberPerRow = 10;

        foreach (var row in rows)
        {
            for (var number = 1; number <= seatNumberPerRow; number++)
            {
                var seatId = Guid.NewGuid();
                var seat = new Seat($"{row}{number}");
                seat.Create();
                await seatRepository.AddSeatAsync(seat);
            }
        }
    }
}
