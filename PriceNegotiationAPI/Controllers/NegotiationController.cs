using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PriceNegotiationAPI.DTOs.Negotiation;
using PriceNegotiationAPI.Exceptions;
using PriceNegotiationAPI.Services;
using System.Net;

namespace PriceNegotiationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationsController : ControllerBase
    {
        private readonly INegotiationService _negotiationService;

        public NegotiationsController(INegotiationService negotiationService)
        {
            _negotiationService = negotiationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNegotiationById(Guid id)
        {
            try
            {
                var negotiation = await _negotiationService.GetNegotiationByIdAsync(id);
                if (negotiation == null)
                {
                    return NotFound();
                }
                return Ok(negotiation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the negotiation.");
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetAllNegotiationsForProduct(Guid productId)
        {
            try
            {
                var negotiations = await _negotiationService.GetAllNegotiationsForProductAsync(productId);
                return Ok(negotiations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving negotiations for the product.");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveNegotiations()
        {
            try
            {
                var activeNegotiations = await _negotiationService.GetActiveNegotiationsAsync();
                return Ok(activeNegotiations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving active negotiations.");
            }
        }

        [HttpPost("propose")]
        public async Task<IActionResult> ProposePrice([FromBody] CreateNegotiationDto createDto)
        {
            try
            {
                var negotiation = await _negotiationService.ProposePriceAsync(createDto);
                return StatusCode((int)HttpStatusCode.Created, negotiation);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred during price proposal.");
            }
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptNegotiation(Guid id)
        {
            try
            {
                var negotiation = await _negotiationService.AcceptNegotiationAsync(id);
                return Ok(negotiation);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while accepting the negotiation.");
            }
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectNegotiation(Guid id)
        {
            try
            {
                var negotiation = await _negotiationService.RejectNegotiationAsync(id);
                return Ok(negotiation);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while rejecting the negotiation.");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelNegotiation(Guid id)
        {
            try
            {
                var negotiation = await _negotiationService.CancelNegotiationAsync(id);
                return Ok(negotiation);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while canceling the negotiation.");
            }
        }
    }
}
