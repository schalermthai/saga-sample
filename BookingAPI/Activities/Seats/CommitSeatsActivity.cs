using MassTransit;
using SBWorkflow.Seats.Domain;

namespace BookingAPI.Activities.Seats;

public class CommitSeatsActivity(IHttpClientFactory httpClientFactory)
    : IActivity<CommitSeatsArguments, CommitSeatsLog>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("SeatService");

    public async Task<ExecutionResult> Execute(ExecuteContext<CommitSeatsArguments> context)
    {
        foreach (var seat in context.Arguments.SelectedSeats)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/seats/{seat}/commit", new { });

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Failed to commit seat {seat}");

            await context.Publish(new SeatCommitted
            {
                SeatLabel = seat,
                Timestamp = DateTime.UtcNow
            });
        }

        return context.CompletedWithVariables(new { SelectedSeats = context.Arguments.SelectedSeats });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<CommitSeatsLog> context)
    {
        foreach (var seat in context.Log.SelectedSeats)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/seats/{seat}/reopen", new { });

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Failed to reopen seat {seat}");

            await context.Publish(new SeatReOpened
            {
                SeatLabel = seat,
                Timestamp = DateTime.UtcNow
            });
        }
        
        return context.Compensated();
    }
}

public interface CommitSeatsArguments
{
    List<string> SelectedSeats { get; }

}

public interface CommitSeatsLog
{
    List<string> SelectedSeats { get; }

}
