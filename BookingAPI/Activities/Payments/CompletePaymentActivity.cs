using MassTransit;

namespace BookingAPI.Activities.Payments;

public class CompletePaymentActivity(IHttpClientFactory httpClientFactory)
    : IActivity<CompletePaymentArguments, CompletePaymentLog>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("PaymentService");

    public async Task<ExecutionResult> Execute(ExecuteContext<CompletePaymentArguments> context)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/payments/{context.Arguments.PaymentId}/complete", new { PaymentId = context.Arguments.PaymentId });

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to complete payment: {response.ReasonPhrase} - {errorContent}");
        }

        await context.Publish(new PaymentCompleted
        {
            PaymentId = context.Arguments.PaymentId,
            BookingId = context.Arguments.BookingId,
            Amount = context.Arguments.Amount,
            Timestamp = DateTime.UtcNow
        });

        return context.Completed();
    }

    public async Task<CompensationResult> Compensate(CompensateContext<CompletePaymentLog> context)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/payments/{context.Log.PaymentId}/cancel", new { PaymentId = context.Log.PaymentId });

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to cancel payment: {response.ReasonPhrase} - {errorContent}");
        }

        await context.Publish(new PaymentCanceled
        {
            PaymentId = context.Log.PaymentId,
            BookingId = context.Log.BookingId,
            Amount = context.Log.Amount,
            Timestamp = DateTime.UtcNow
        });

        return context.Compensated();
    }
}

public interface CompletePaymentArguments
{
    Guid PaymentId { get; }
    Guid BookingId { get; }
    decimal Amount { get; }
}

public class CompletePaymentLog
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentCompleted
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}

public class PaymentCanceled
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}