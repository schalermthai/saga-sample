using Microsoft.AspNetCore.Mvc;
using SBWorkflow.Seats.Domain;

namespace SBWorkflow.Seats.Controller
{
    [ApiController]
    [Route("api/seats")]
    public class SeatsInventoryController(ISeatRepository seatRepository) : ControllerBase
    {
        [HttpPost("{label}/reserve")]
        public async Task<IActionResult> ReserveSeat(string label)
        {
            var seat = await seatRepository.GetSeatAsync(label);
            if (seat == null)
            {
                return NotFound($"Seat {label} not found.");
            }

            try
            {
                seat.Reserve();
                await seatRepository.UpdateSeatAsync(seat);
                return Ok(new { Success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{label}/release")]
        public async Task<IActionResult> ReleaseSeat(string label)
        {
            var seat = await seatRepository.GetSeatAsync(label);
            if (seat == null)
            {
                return NotFound($"Seat {label} not found.");
            }

            try
            {
                seat.Release();
                await seatRepository.UpdateSeatAsync(seat);
                return Ok(new { Success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{label}/commit")]
        public async Task<IActionResult> CommitSeat(string label)
        {
            var seat = await seatRepository.GetSeatAsync(label);
            if (seat == null)
            {
                return NotFound($"Seat {label} not found.");
            }

            try
            {
                seat.Commit();
                await seatRepository.UpdateSeatAsync(seat);
                return Ok(new { Success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{label}/reopen")]
        public async Task<IActionResult> ReOpenSeat(string label)
        {
            var seat = await seatRepository.GetSeatAsync(label);
            if (seat == null)
            {
                return NotFound($"Seat {label} not found.");
            }

            try
            {
                seat.ReOpen();
                await seatRepository.UpdateSeatAsync(seat);
                return Ok(new { Success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{label}/checkin")]
        public async Task<IActionResult> CheckInSeat(string label)
        {
            var seat = await seatRepository.GetSeatAsync(label);
            if (seat == null)
            {
                return NotFound($"Seat {label} not found.");
            }

            try
            {
                seat.CheckIn();
                await seatRepository.UpdateSeatAsync(seat);
                return Ok(new { Success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{label}")]
        public async Task<IActionResult> GetSeatByLabel(string label)
        {
            var seat = await seatRepository.GetSeatAsync(label);
            if (seat == null)
            {
                return NotFound($"Seat with label {label} not found.");
            }

            return Ok(new 
            {
                Id = seat.SeatLabel,
                State = seat.State.ToString()
            });
        }
    }

}
