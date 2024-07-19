using MassTransit;

namespace SBWorkflow;

public class BookingActivity : IStateMachineActivity<BookingState, PaymentRequestedEvent>
{
    public void Probe(ProbeContext context)
    {
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<BookingState, PaymentRequestedEvent> context, IBehavior<BookingState, PaymentRequestedEvent> next)
    {
        await context.Publish(new PaymentCompletedEvent());
        await next.Execute(context);
    }

    public async Task Faulted<TException>(BehaviorExceptionContext<BookingState, PaymentRequestedEvent, TException> context, IBehavior<BookingState, PaymentRequestedEvent> next) where TException : Exception
    {
        await context.Publish(new PaymentFailedEvent());
        await next.Faulted(context);
    }
}