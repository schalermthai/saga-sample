
```mermaid
stateDiagram-v2
    [*] --> Created: BookingCreatedEvent
    Created --> MovieSelected: MovieSelectedEvent
    Created --> BookingCancelled: BookingCancelledEvent
    MovieSelected --> MovieSelected: MovieSelectedEvent
    MovieSelected --> SeatsSelected: SeatsSelectedEvent \n[All seats selected]
    MovieSelected --> MovieSelected: SeatsSelectedEvent \n[Not all seats selected]
    MovieSelected --> BookingCancelled: BookingCancelledEvent
    SeatsSelected --> SeatsSelected: SeatsSelectedEvent \n[All seats selected]
    SeatsSelected --> MovieSelected: SeatsSelectedEvent \n[Not all seats selected]
    SeatsSelected --> ExtrasAdded: ExtrasAddedEvent
    SeatsSelected --> BookingCancelled: BookingCancelledEvent
    ExtrasAdded --> ExtrasAdded: ExtrasAddedEvent
    ExtrasAdded --> PaymentSubmitted: PaymentRequestedEvent
    PaymentSubmitted --> BookingCompleted: PaymentCompletedEvent
    PaymentSubmitted --> BookingFailed: PaymentFailedEvent
    BookingCompleted --> [*]
    BookingFailed --> [*]
    BookingCancelled --> [*]