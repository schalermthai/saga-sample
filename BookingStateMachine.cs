namespace SBWorkflow;

using System;
using MassTransit;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public State Create { get; private set; }
    public State SelectingMovie { get; private set; }
    public State SelectingSeats { get; private set; }
    public State AddingExtras { get; private set; }
    public State Committing { get; private set; }
    public State ProcessingPayment { get; private set; }
    public State CancellingBooking { get; private set; }


    public Event<OrderCreatedEvent> BookingCreatedEvent { get; private set; }
    public Event<MovieSelectedEvent> MovieSelectedEvent { get; private set; }
    public Event<SeatsSelectedEvent> SeatsSelectedEvent { get; private set; }
    public Event<ExtrasAddedEvent> ExtrasAddedEvent { get; private set; }
    public Event<BookingCommitedEvent> BookingCommitedEvent { get; private set; }
    public Event<PaymentCompletedEvent> PaymentProcessedEvent { get; private set; }
    public Event<BookingCancelledEvent> BookingCancelledEvent { get; private set; }

    public BookingStateMachine()
    {
        
        InstanceState(x => x.CurrentState);
        
        Event(() => BookingCreatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => MovieSelectedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => SeatsSelectedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ExtrasAddedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => BookingCommitedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentProcessedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
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
                .TransitionTo(Committing),
            When(BookingCancelledEvent)
                .TransitionTo(CancellingBooking)
        );
        
        During(Committing,
            When(BookingCommitedEvent)
                .Then(context =>
                {
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .TransitionTo(ProcessingPayment)
        );

        During(ProcessingPayment,
            When(PaymentProcessedEvent)
                .Then(context =>
                {
                    context.Saga.PaymentStatus = context.Message.PaymentStatus;
                    context.Saga.LastUpdated = DateTime.UtcNow;
                })
                .Finalize()
        );
    }
}
