using System.Collections.Concurrent;
using SBWorkflow.Seats.Domain;

namespace SeatsInventory.Repository;

public class InMemorySeatRepository : ISeatRepository
{
    private readonly ConcurrentDictionary<string, Seat> _seats = new();

    public Task<Seat?> GetSeatAsync(string seatLabel)
    {
        _seats.TryGetValue(seatLabel, out var seat);
        return Task.FromResult(seat);
    }

    public Task AddSeatAsync(Seat seat)
    {
        _seats[seat.SeatLabel] = seat;
        return Task.CompletedTask;
    }

    public Task UpdateSeatAsync(Seat seat)
    {
        _seats[seat.SeatLabel] = seat;
        return Task.CompletedTask;
    }

    public Task<bool> SeatExistsAsync(string seatLabel)
    {
        return Task.FromResult(_seats.ContainsKey(seatLabel));
    }
}
