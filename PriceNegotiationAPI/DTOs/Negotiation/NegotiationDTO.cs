using PriceNegotiationAPI.DTOs.Product;
using PriceNegotiationAPI.Enums;

namespace PriceNegotiationAPI.DTOs.Negotiation
{
    public class NegotiationDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal ProposedPrice { get; set; }
        public NegotiationStatus Status { get; set; }
        public DateTime ProposedDate { get; set; }
        public int AttemptsCount { get; set; }
        public DateTime? LastRejectionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
