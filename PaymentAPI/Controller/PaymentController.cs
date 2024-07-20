using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Domain;

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
                payment.Authorize();
                if (payment.State == PaymentState.Failed)
                {
                    return BadRequest("Payment creation failed due to amount exceeding limit.");
                }
                
                await paymentRepository.CreateAsync(payment);
                await publishEndpoint.Publish(new PaymentCreated(payment));
                
                return Ok(Dto(payment));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{paymentId}/complete")]
        public async Task<IActionResult> CompletePayment(Guid paymentId)
        {
            var payment = await paymentRepository.GetAsync(paymentId);
            if (payment == null)
            {
                return NotFound($"Payment {paymentId} not found.");
            }

            try
            {
                payment.Complete();
                await paymentRepository.UpdateAsync(payment);
                await publishEndpoint.Publish(new PaymentCompleted(payment));

                return Ok(Dto(payment));

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{paymentId}/cancel")]
        public async Task<IActionResult> CancelPayment(Guid paymentId)
        {
            var payment = await paymentRepository.GetAsync(paymentId);
            if (payment == null)
            {
                return NotFound($"Payment {paymentId} not found.");
            }

            try
            {
                payment.Cancel();
                await paymentRepository.UpdateAsync(payment);
                await publishEndpoint.Publish(new PaymentCanceled(payment));
                
                return Ok(Dto(payment));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentStatus(Guid paymentId)
        {
            var payment = await paymentRepository.GetAsync(paymentId);
            if (payment == null)
            {
                return NotFound($"Payment with ID {paymentId} not found.");
            }
            
            return Ok(Dto(payment));
        }
        
        private object? Dto(Payment payment)
        {
            return
                new
                {
                    Success = true,
                    PaymentId = payment.PaymentId,
                    BookingId = payment.BookingId,
                    Status = payment.State.ToString()
                };
        }
    }
    public class CreatePaymentRequest
    {
        public Guid BookingId { get; set; }
        public decimal Amount { get; set; }
    }


}
