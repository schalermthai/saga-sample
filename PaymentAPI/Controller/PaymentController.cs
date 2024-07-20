using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SBWorkflow.Payments.Domain;

namespace PaymentAPI.Controller
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController(IPaymentRepository paymentRepository, IPublishEndpoint publishEndpoint)
        : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var payment = new Payment(request.BookingId, request.Amount);

            try
            {
                await paymentRepository.AddPaymentAsync(payment);
                if (payment.State == PaymentState.Failed)
                {
                    return BadRequest("Payment creation failed due to amount exceeding limit.");
                }

                return Ok(new { Success = true, PaymentId = payment.PaymentId, Status = payment.State.ToString() });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<IActionResult> CompletePayment(Guid paymentId)
        {
            var payment = await paymentRepository.GetPaymentAsync(paymentId);
            if (payment == null)
            {
                return NotFound($"Payment {paymentId} not found.");
            }

            try
            {
                payment.Complete();
                await paymentRepository.UpdatePaymentAsync(payment);

                return Ok(new { Success = true, PaymentId = payment.PaymentId, Status = payment.State.ToString() });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{paymentId}/cancel")]
        public async Task<IActionResult> CancelPayment(Guid paymentId)
        {
            var payment = await paymentRepository.GetPaymentAsync(paymentId);
            if (payment == null)
            {
                return NotFound($"Payment {paymentId} not found.");
            }

            try
            {
                payment.Cancel();
                await paymentRepository.UpdatePaymentAsync(payment);
                return Ok(new { Success = true, PaymentId = payment.PaymentId, Status = payment.State.ToString() });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentStatus(Guid paymentId)
        {
            var payment = await paymentRepository.GetPaymentAsync(paymentId);
            if (payment == null)
            {
                return NotFound($"Payment with ID {paymentId} not found.");
            }

            var paymentDto = new PaymentDto
            {
                PaymentId = payment.PaymentId,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                State = payment.State.ToString()
            };

            return Ok(paymentDto);
        }
    }

    public class CreatePaymentRequest
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
    }
}
