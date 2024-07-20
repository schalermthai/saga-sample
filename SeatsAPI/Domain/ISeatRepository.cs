namespace SBWorkflow.Seats.Domain;

using System.Threading.Tasks;

public interface ISeatRepository
{
    Task<Seat?> GetSeatAsync(string seatLabel);
    Task AddSeatAsync(Seat seat);
    Task UpdateSeatAsync(Seat seat);
    Task<bool> SeatExistsAsync(string seatLabel);
}
