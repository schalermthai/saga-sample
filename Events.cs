namespace SBWorkflow;


public class OrderCreatedEvent
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

public class BookingCommitedEvent
{
    public Guid CorrelationId { get; set; }
}

public class PaymentCompletedEvent
{
    public Guid CorrelationId { get; set; }
    public string PaymentStatus { get; set; }
}

public class BookingCancelledEvent
{
    public Guid CorrelationId { get; set; }
}
