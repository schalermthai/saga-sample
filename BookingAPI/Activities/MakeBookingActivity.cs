using MassTransit;
using SBWorkflow.Booking.Domain;

namespace SBWorkflow.Booking.Activities;

public class MakeBookingActivity : IStateMachineActivity<BookingState, PaymentRequestedEvent>
{
    public void Probe(ProbeContext context) { }

    public void Accept(StateMachineVisitor visitor) { }

    public async Task Execute(BehaviorContext<BookingState, PaymentRequestedEvent> context, IBehavior<BookingState, PaymentRequestedEvent> next)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());

        // Add ReserveSeatActivity
        builder.AddActivity("ReserveSeatsActivity", new Uri("queue:ReserveSeats_execute"), new
        {
            SelectedSeats = context.Saga.SelectedSeats,
            BookingId = context.Saga.CorrelationId
        });

        // Add CreatePaymentActivity
        builder.AddActivity("CreatePaymentActivity", new Uri("queue:CreatePayment_execute"), new
        {
            BookingId = context.Saga.CorrelationId,
            Amount = context.Saga.Amount
        });

        // Add CommitSeatActivity
        builder.AddActivity("CommitSeatsActivity", new Uri("queue:CommitSeats_execute"), new
        {
            SelectedSeats = context.Saga.SelectedSeats,
            BookingId = context.Saga.CorrelationId
        });

        var routingSlip = builder.Build();

        await context.Execute(routingSlip);

        await context.Publish(new PaymentCompletedEvent() { CorrelationId = context.Saga.CorrelationId });

        await next.Execute(context);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<BookingState, PaymentRequestedEvent, TException> context, IBehavior<BookingState, PaymentRequestedEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}