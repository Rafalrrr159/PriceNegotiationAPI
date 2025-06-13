using PriceNegotiationAPI.Enums;
using PriceNegotiationAPI.Interfaces;
using PriceNegotiationAPI.Models;

namespace PriceNegotiationAPI.Repositories
{
    public class InMemoryNegotiationRepository : INegotiationRepository
    {
        private static readonly List<Negotiation> _negotiations = new List<Negotiation>();

        public Task<Negotiation?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_negotiations.FirstOrDefault(n => n.Id == id));
        }

        public Task<List<Negotiation>> GetAllByProductIdAsync(Guid productId)
        {
            return Task.FromResult(_negotiations.Where(n => n.ProductId == productId).ToList());
        }

        public Task<Negotiation?> GetActiveNegotiationByClientIdAndProductIdAsync(Guid clientId, Guid productId)
        {
            return Task.FromResult(_negotiations.FirstOrDefault(n =>
                n.ClientId == clientId &&
                n.ProductId == productId &&
                (n.Status == NegotiationStatus.Proposed || n.Status == NegotiationStatus.Rejected)));
        }

        public Task AddAsync(Negotiation negotiation)
        {
            _negotiations.Add(negotiation);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Negotiation negotiation)
        {
            var existingNegotiation = _negotiations.FirstOrDefault(n => n.Id == negotiation.Id);
            if (existingNegotiation != null)
            {
                existingNegotiation.ProposedPrice = negotiation.ProposedPrice;
                existingNegotiation.Status = negotiation.Status;
                existingNegotiation.AttemptsCount = negotiation.AttemptsCount;
                existingNegotiation.LastRejectionDate = negotiation.LastRejectionDate;
                existingNegotiation.LastModifiedDate = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _negotiations.RemoveAll(n => n.Id == id);
            return Task.CompletedTask;
        }
    }
}
