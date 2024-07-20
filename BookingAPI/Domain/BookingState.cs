using MassTransit;

namespace BookingAPI.Domain;

public class BookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public DateTime Created { get; set; }
    public int NumberOfSeats { get; set; }
    public DateTime LastUpdated { get; set; }
    public string SelectedMovie { get; set; }
    public HashSet<string> SelectedSeats { get; set; } = new();
    public List<string> SelectedExtras { get; set; } = new();
    public string PaymentStatus { get; set; }
    public decimal Amount { get; set; }
    
    public void OnCreated(BookingCreatedEvent message)
    {
        CorrelationId = message.CorrelationId;
        NumberOfSeats = message.NumberOfSeats;
        Created = DateTime.UtcNow;
    }
    public void OnSelectedMovie(MovieSelectedEvent message)
    {
        SelectedMovie = message.MovieId;
        LastUpdated = DateTime.UtcNow;
    }

    public void OnSelectedSeats(SeatsSelectedEvent message)
    {
        SelectedSeats.UnionWith(message.SelectedSeats);
        LastUpdated = DateTime.UtcNow;
    }

    public void OnSelectedExtras(ExtrasAddedEvent message)
    {
        SelectedExtras = message.SelectedExtras;
        LastUpdated = DateTime.UtcNow;
    }

    public void OnPaymentCompleted(PaymentCompletedEvent message)
    {
        PaymentStatus = "Success";
        LastUpdated = DateTime.UtcNow;
    }
    
    public void OnPaymentFailed(PaymentFailedEvent message)
    {
        PaymentStatus = "Failed";
        LastUpdated = DateTime.UtcNow;
    }
}