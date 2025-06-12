using System.ComponentModel.DataAnnotations;

namespace PriceNegotiationAPI.DTOs.Negotiation
{
    public class CreateNegotiationDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Proposed price must be greater than zero.")]
        public decimal ProposedPrice { get; set; }
    }
}
