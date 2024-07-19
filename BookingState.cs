namespace SBWorkflow;

using System;
using MassTransit;

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
}