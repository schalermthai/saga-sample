using BookingAPI.Domain;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace BookingAPI.Controller;

[ApiController]
[Route("api/order")]
public class BookingController(ILoadSagaRepository<BookingState> inMemorySagaRepository, IPublishEndpoint publishEndpoint)
    : ControllerBase
{
    [HttpPost("/")]
    public async Task<IActionResult> CreateOrder(CreateRequest request)
    {
        var orderId = Guid.NewGuid();
        await publishEndpoint.Publish(new BookingCreatedEvent() { CorrelationId = orderId, NumberOfSeats = request.NumberOfSeats });

        return Ok(orderId);
    }

    [HttpPost("/{orderId}/movie")]
    public async Task<IActionResult> SelectMovie(Guid orderId, MovieRequest request)
    {
        await publishEndpoint.Publish(new MovieSelectedEvent() { CorrelationId = orderId, MovieId = request.MovieId});
        return Ok();
    }

    [HttpPost("/{orderId}/seats")]
    public async Task<IActionResult> SelectSeats(Guid orderId, SeatsRequest request)
    {
        await publishEndpoint.Publish(new SeatsSelectedEvent() { CorrelationId = orderId, SelectedSeats = request.Seats});
        return Ok();
    }
    
    [HttpPost("/{orderId}/extras")]
    public async Task<IActionResult> SelectExtras(Guid orderId, ExtrasRequest request)
    {
        await publishEndpoint.Publish(new ExtrasAddedEvent() { CorrelationId = orderId, SelectedExtras = request.Extras});
        return Ok();
    }
    
    [HttpPost("/{orderId}/payment")]
    public async Task<IActionResult> Payment(Guid orderId, PaymentRequest request)
    {
        await publishEndpoint.Publish(new PaymentSubmittedEvent() { CorrelationId = orderId, Amount = request.Amount});
        return Ok();
    }
    
    [HttpPost("/{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        await publishEndpoint.Publish(new BookingCancelledEvent() { CorrelationId = orderId });
        return Ok();
    }

    [HttpGet("/{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var sagaInstance = await inMemorySagaRepository.Load(orderId);
        if (sagaInstance == null)
            return NotFound();

        return Ok(sagaInstance);
    }
}

public class CreateRequest
{
    public int NumberOfSeats { get; set; } = 1;
}

public class ExtrasRequest
{
    public List<string> Extras { get; set; }
}

public class SeatsRequest
{
    public List<string> Seats { get; set; }
}

public class MovieRequest
{
    public string MovieId { get; set; }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
}