namespace PaymentAPI.Domain;

public interface IPaymentRepository
{
    Task<Payment?> GetAsync(Guid paymentId);
    Task CreateAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task<bool> PaymentExistsAsync(Guid paymentId);
}