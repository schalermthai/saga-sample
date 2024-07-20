namespace SBWorkflow.Payments.Domain;
using System;

public class Payment(Guid bookingId, decimal amount)
{
    public Guid PaymentId { get; private set; } = Guid.NewGuid();
    public Guid BookingId { get; private set; } = bookingId;
    public PaymentState State { get; private set; } = amount > 100 ? PaymentState.Failed : PaymentState.Created;
    public decimal Amount { get; private set; } = amount;

    public void Authorize()
    {
        if (State != PaymentState.Created)
            throw new InvalidOperationException("Payment can only be completed from the Created state.");

        State = Amount > 100 ? PaymentState.Failed : PaymentState.Authorized;
    }

    public void Complete()
    {
        if (State != PaymentState.Created)
            throw new InvalidOperationException("Payment can only be completed from the Created state.");

        State = PaymentState.Completed;
    }

    public void Cancel()
    {
        if (State != PaymentState.Created)
            throw new InvalidOperationException("Payment can only be canceled from the Created state.");

        State = PaymentState.Canceled;
    }
}


public enum PaymentState
{
    Created,
    Authorized,
    Completed,
    Canceled,
    Failed
}
