namespace SBWorkflow;

using System;
using MassTransit;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public State SelectingMovie { get; } = null!;
    public State SelectingSeats { get; } = null!;
    public State AddingExtras { get; } = null!;
    public State RequestingPayment { get; } = null!;
    public State ProcessingPayment { get; } = null!;
    public State Completed { get; } = null!;
    public State Failed { get; } = null!;
    public State CancellingBooking { get; } = null!;

    public Event<OrderCreatedEvent> BookingCreatedEvent { get; private set; }
    public Event<MovieSelectedEvent> MovieSelectedEvent { get; private set; }
    public Event<SeatsSelectedEvent> SeatsSelectedEvent { get; private set; }
    public Event<ExtrasAddedEvent> ExtrasAddedEvent { get; private set; }
    public Event<PaymentRequestedEvent> PaymentRequestedEvent { get; private set; }
    public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; private set; }
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; private set; }
    public Event<BookingCancelledEvent> BookingCancelledEvent { get; private set; }

    public BookingStateMachine()
    {
        
        InstanceState(x => x.CurrentState);
        
        Event(() => BookingCreatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => MovieSelectedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => SeatsSelectedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ExtrasAddedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentRequestedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => BookingCancelledEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(BookingCreatedEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.NumberOfSeats = context.Message.NumberOfSeats;
                    context.Saga.Created = DateTime.UtcNow;
                })
                .TransitionTo(SelectingMovie)
        );

        During(SelectingMovie,
            When(MovieSelectedEvent)
                .Then(context =>
                {
                    context.Saga.SelectedMovie = context.Message.MovieId;
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .TransitionTo(SelectingSeats),
            When(BookingCancelledEvent)
                .TransitionTo(CancellingBooking)
        );
        
        During(SelectingSeats,
            When(SeatsSelectedEvent)
                .Then(context =>
                {
                    context.Saga.SelectedSeats.UnionWith(context.Message.SelectedSeats);
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .If(context => context.Saga.SelectedSeats.Count == context.Saga.NumberOfSeats, binder => binder.TransitionTo(AddingExtras))
                .If(context => context.Saga.SelectedSeats.Count < context.Saga.NumberOfSeats, binder => binder.TransitionTo(SelectingSeats)),
            When(BookingCancelledEvent)
                .TransitionTo(CancellingBooking)
        );
        
        During(AddingExtras,
            When(ExtrasAddedEvent)
                .Then(context =>
                {
                    context.Saga.SelectedExtras = context.Message.SelectedExtras;
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .TransitionTo(RequestingPayment),
            When(BookingCancelledEvent)
                .TransitionTo(CancellingBooking)
        );

        During(RequestingPayment,
            When(PaymentRequestedEvent)
                .Activity(context => context.OfType<BookingActivity>())
                .TransitionTo(ProcessingPayment)
        );

        During(ProcessingPayment, 
            When(PaymentCompletedEvent)
                .Then(context =>
                {
                    context.Saga.PaymentStatus = "Success";
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .TransitionTo(Completed)
                .Finalize(),
            When(PaymentFailedEvent)
                .Then(context =>
                {
                    context.Saga.PaymentStatus = "Failed";
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .TransitionTo(Failed)
                .Finalize()
            );

    }
}
