using System.Collections.Concurrent;
using PaymentAPI.Domain;

namespace PaymentAPI.Repository;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<string, Payment> _payments = new ();

    public Task<Payment?> GetAsync(Guid paymentId)
    {
        _payments.TryGetValue(paymentId.ToString(), out var payment);
        return Task.FromResult(payment);
    }

    public Task CreateAsync(Payment payment)
    {
        _payments[payment.PaymentId.ToString()] = payment;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Payment payment)
    {
        _payments[payment.PaymentId.ToString()] = payment;
        return Task.CompletedTask;
    }

    public Task<bool> PaymentExistsAsync(Guid paymentId)
    {
        return Task.FromResult(_payments.ContainsKey(paymentId.ToString()));
    }
}


