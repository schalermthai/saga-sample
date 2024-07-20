using MassTransit;
using SBWorkflow.Seats.Domain;

namespace BookingAPI.Activities.Seats;

public class ReserveSeatsActivity(IHttpClientFactory httpClientFactory) : IActivity<ReserveSeatsArguments, ReserveSeatsLog>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("SeatService");

    public async Task<ExecutionResult> Execute(ExecuteContext<ReserveSeatsArguments> context)
    {
        foreach (var seat in context.Arguments.SelectedSeats)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/seats/{seat}/reserve", new { });

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Failed to reserve seat {seat}");
            
            await context.Publish(new SeatReserved
            {
                SeatLabel = seat,
                Timestamp = DateTime.UtcNow
            });
        }

        return context.CompletedWithVariables(new { SelectedSeats = context.Arguments.SelectedSeats});
    }

    public async Task<CompensationResult> Compensate(CompensateContext<ReserveSeatsLog> context)
    {
        foreach (var seat in context.Log.SelectedSeats)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/seats/{seat}/release", new { });

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Failed to release seat {seat}");

            await context.Publish(new SeatReleased
            {
                SeatLabel = seat,
                Timestamp = DateTime.UtcNow
            });
        }

        return context.Compensated();
    }
}

public interface ReserveSeatsArguments
{
    List<string> SelectedSeats { get; }
}

public interface ReserveSeatsLog
{
    List<string> SelectedSeats { get; }
}
