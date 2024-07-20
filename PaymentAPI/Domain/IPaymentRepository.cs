namespace SBWorkflow.Payments.Domain;

public interface IPaymentRepository
{
    Task<Payment?> GetPaymentAsync(Guid paymentId);
    Task AddPaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
    Task<bool> PaymentExistsAsync(Guid paymentId);
}