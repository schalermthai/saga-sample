namespace BookingAPI.Domain;


public class BookingCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public int NumberOfSeats { get; set; }
}
public class MovieSelectedEvent
{
    public Guid CorrelationId { get; set; }
    public string MovieId { get; set; }
}

public class SeatsSelectedEvent
{
    public Guid CorrelationId { get; set; }
    public List<string> SelectedSeats { get; set; }
}

public class ExtrasAddedEvent
{
    public Guid CorrelationId { get; set; }
    public List<string> SelectedExtras { get; set; }
    // Add properties for extras as needed
}

public class PaymentSubmittedEvent
{
    public Guid CorrelationId { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentCompletedEvent
{
    public Guid CorrelationId { get; set; }
}

public class PaymentFailedEvent
{
    public Guid CorrelationId { get; set; }
}
public class BookingCancelledEvent
{
    public Guid CorrelationId { get; set; }
}
