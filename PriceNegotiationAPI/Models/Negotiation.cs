using PriceNegotiationAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace PriceNegotiationAPI.Models
{
    public class Negotiation
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Proposed price must be greater than zero.")]
        public decimal ProposedPrice { get; set; }

        public DateTime ProposedDate { get; set; }

        public NegotiationStatus Status { get; set; }

        public int AttemptsCount { get; set; }

        public DateTime? LastRejectionDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedDate { get; set; }

        private Negotiation() { }

        public Negotiation(Guid productId, decimal initialProposedPrice)
        {
            if (initialProposedPrice <= 0)
            {
                throw new ArgumentException("Proposed price must be greater than zero.", nameof(initialProposedPrice));
            }

            Id = Guid.NewGuid();
            ProductId = productId;
            ProposedPrice = initialProposedPrice;
            ProposedDate = DateTime.UtcNow;
            Status = NegotiationStatus.Proposed;
            AttemptsCount = 1;
            CreatedDate = DateTime.UtcNow;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}
