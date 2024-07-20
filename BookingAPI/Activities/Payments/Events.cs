namespace SBWorkflow.Payments.Domain;

using System;

public class PaymentCreated
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
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
