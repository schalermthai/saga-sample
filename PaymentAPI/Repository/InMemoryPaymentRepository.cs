using SBWorkflow.Payments.Domain;

namespace SBWorkflow.Payments.Repository;

using System.Collections.Concurrent;
using System.Threading.Tasks;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<string, Payment> _payments = new ();

    public Task<Payment?> GetPaymentAsync(Guid paymentId)
    {
        _payments.TryGetValue(paymentId.ToString(), out var payment);
        return Task.FromResult(payment);
    }

    public Task AddPaymentAsync(Payment payment)
    {
        _payments[payment.PaymentId.ToString()] = payment;
        return Task.CompletedTask;
    }

    public Task UpdatePaymentAsync(Payment payment)
    {
        _payments[payment.PaymentId.ToString()] = payment;
        return Task.CompletedTask;
    }

    public Task<bool> PaymentExistsAsync(Guid paymentId)
    {
        return Task.FromResult(_payments.ContainsKey(paymentId.ToString()));
    }
}


