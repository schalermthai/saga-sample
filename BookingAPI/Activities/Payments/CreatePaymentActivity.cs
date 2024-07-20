using MassTransit;

namespace SBWorkflow.Payments.Activities;

public class CreatePaymentActivity(IHttpClientFactory httpClientFactory)
    : IActivity<CreatePaymentArguments, CreatePaymentLog>
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("PaymentService");

    public async Task<ExecutionResult> Execute(ExecuteContext<CreatePaymentArguments> context)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/payments/create", new
        {
            BookingId = context.Arguments.BookingId,
            Amount = context.Arguments.Amount
        });

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to create payment: {response.ReasonPhrase} - {errorContent}");
        }

        var payment = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();

        if (payment.Status == "Failed")
        {
            throw new InvalidOperationException($"Payment creation failed due to amount exceeding limit.");
        }

        await context.Publish(new PaymentCreated
        {
            PaymentId = payment.PaymentId,
            BookingId = context.Arguments.BookingId,
            Amount = context.Arguments.Amount,
            Timestamp = DateTime.UtcNow
        });

        return context.CompletedWithVariables(new CreatePaymentLog { PaymentId = payment.PaymentId });
    }

    public Task<CompensationResult> Compensate(CompensateContext<CreatePaymentLog> context)
    {
        // No compensation required for payment creation
        return Task.FromResult(context.Compensated());
    }
}

public interface CreatePaymentArguments
{
    Guid BookingId { get; }
    decimal Amount { get; }
}

public class CreatePaymentLog
{
    public Guid PaymentId { get; set; }
}

public class CreatePaymentResponse
{
    public Guid PaymentId { get; set; }
    public string Status { get; set; }
}

public class PaymentCreated
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}