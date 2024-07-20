using BookingAPI.Activities.Booking;
using BookingAPI.Domain;
using MassTransit;

namespace BookingAPI.StateMachine;

public class BookingStateMachine : MassTransitStateMachine<BookingState>
{
    public State Created { get; } = null!;
    public State MovieSelected { get; } = null!;
    public State SeatsSelected { get; } = null!;
    public State ExtrasAdded { get; } = null!;
    public State PaymentSubmitted { get; } = null!;
    public State BookingCompleted { get; } = null!;
    public State BookingFailed { get; } = null!;
    public State BookingCancelled { get; } = null!;

    public Event<BookingCreatedEvent> BookingCreatedEvent { get; private set; }
    public Event<MovieSelectedEvent> MovieSelectedEvent { get; private set; }
    public Event<SeatsSelectedEvent> SeatsSelectedEvent { get; private set; }
    public Event<ExtrasAddedEvent> ExtrasAddedEvent { get; private set; }
    public Event<PaymentSubmittedEvent> PaymentSubmittedEvent { get; private set; }
    public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; private set; }
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; private set; }
    public Event<BookingCancelledEvent> BookingCancelledEvent { get; private set; }

    public BookingStateMachine()
    {
        InstanceState(x => x.CurrentState);
        SetupEventsCorrelation();
        SetupStateTransitions();
    }
    
    private void SetupEventsCorrelation()
    {
        Event(() => BookingCreatedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => MovieSelectedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => SeatsSelectedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => ExtrasAddedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentSubmittedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => PaymentFailedEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
        Event(() => BookingCancelledEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
    }

    private void SetupStateTransitions()
    {
        Initially(
    When(BookingCreatedEvent)
        .Then(context => context.Saga.OnCreated(context.Message))
        .TransitionTo(Created)
        );

        During(Created,
            When(MovieSelectedEvent)
                .Then(context => context.Saga.OnSelectedMovie(context.Message))
                .TransitionTo(MovieSelected),
            When(BookingCancelledEvent)
                .TransitionTo(BookingCancelled)
        );

        During(MovieSelected,
            When(MovieSelectedEvent)
                .Then(context => context.Saga.OnSelectedMovie(context.Message))
                .TransitionTo(MovieSelected),
            When(SeatsSelectedEvent)
                .Then(context => context.Saga.OnSelectedSeats(context.Message))
                .IfElse(context => context.Saga.SelectedSeats.Count == context.Saga.NumberOfSeats, 
                    binder => binder.TransitionTo(SeatsSelected), 
                    binder => binder.TransitionTo(MovieSelected)),
            When(BookingCancelledEvent)
                .TransitionTo(BookingCancelled)
        );

        During(SeatsSelected,
            When(SeatsSelectedEvent)
                .Then(context => context.Saga.OnSelectedSeats(context.Message))
                .IfElse(context => context.Saga.SelectedSeats.Count == context.Saga.NumberOfSeats, 
                    binder => binder.TransitionTo(SeatsSelected), 
                    binder => binder.TransitionTo(MovieSelected)),
            When(ExtrasAddedEvent)
                .Then(context => context.Saga.OnSelectedExtras(context.Message))
                .TransitionTo(ExtrasAdded),
            When(BookingCancelledEvent)
                .TransitionTo(BookingCancelled)
        );

        During(ExtrasAdded,
            When(ExtrasAddedEvent)
                .Then(context => context.Saga.OnSelectedExtras(context.Message))
                .TransitionTo(ExtrasAdded),
            When(PaymentSubmittedEvent)
                .Activity(context => context.OfType<MakeBookingActivity>())
                .TransitionTo(PaymentSubmitted)
        );

        During(PaymentSubmitted, 
            When(PaymentCompletedEvent)
                .Then(context => context.Saga.OnPaymentCompleted(context.Message))
                .TransitionTo(BookingCompleted)
                .Finalize(),
            When(PaymentFailedEvent)
                .Then(context => context.Saga.OnPaymentFailed(context.Message))
                .TransitionTo(BookingFailed)
                .Finalize()
        );
    }
}
