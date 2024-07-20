namespace PaymentAPI.Domain;

public class PaymentCreated(Payment payment)
{
    public Guid PaymentId { get; set; } = payment.PaymentId;
    public Guid BookingId { get; set; } = payment.BookingId;
    public decimal Amount { get; set; } = payment.Amount;
    public DateTime Timestamp { get; set; }= DateTime.UtcNow;
}

public class PaymentCompleted(Payment payment)
{
    public Guid PaymentId { get; set; } = payment.PaymentId;
    public Guid BookingId { get; set; } = payment.BookingId;
    public decimal Amount { get; set; } = payment.Amount;
    public DateTime Timestamp { get; set; }= DateTime.UtcNow;
}

public class PaymentCanceled(Payment payment)
{
    public Guid PaymentId { get; set; } = payment.PaymentId;
    public Guid BookingId { get; set; } = payment.BookingId;
    public decimal Amount { get; set; } = payment.Amount;
    public DateTime Timestamp { get; set; }= DateTime.UtcNow;
}
