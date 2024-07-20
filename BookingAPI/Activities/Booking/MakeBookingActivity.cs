using BookingAPI.Domain;
using MassTransit;
using MassTransit.Courier.Contracts;

namespace BookingAPI.Activities.Booking;

public class MakeBookingActivity : IStateMachineActivity<BookingState, PaymentSubmittedEvent>
{
    public void Probe(ProbeContext context) { }

    public void Accept(StateMachineVisitor visitor) { }

    public async Task Execute(BehaviorContext<BookingState, PaymentSubmittedEvent> context, IBehavior<BookingState, PaymentSubmittedEvent> next)
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
        
        // Subscription for every event in the routing slip
        builder.AddSubscription(new Uri("queue:routing-slip-events"), RoutingSlipEvents.All);

        // Subscription specifically for the completed event
        await builder.AddSubscription(new Uri("queue:routing-slip-completed"), RoutingSlipEvents.Completed, x => x.Send(new PaymentCompletedEvent() { CorrelationId = context.Saga.CorrelationId }));

        var routingSlip = builder.Build();

        await context.Execute(routingSlip);

        // await context.Publish(new PaymentCompletedEvent() { CorrelationId = context.Saga.CorrelationId });

        await next.Execute(context);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<BookingState, PaymentSubmittedEvent, TException> context, IBehavior<BookingState, PaymentSubmittedEvent> next) where TException : Exception
    {
        await next.Faulted(context);
    }
}