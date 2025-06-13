using PriceNegotiationAPI.Models;

namespace PriceNegotiationAPI.Interfaces
{
    public interface INegotiationRepository
    {
        Task<Negotiation?> GetByIdAsync(Guid id);
        Task<List<Negotiation>> GetAllByProductIdAsync(Guid productId);
        Task<List<Negotiation>> GetNegotiationsByClientIdAndProductIdAsync(Guid clientId, Guid productId);
        Task AddAsync(Negotiation negotiation);
        Task UpdateAsync(Negotiation negotiation);
        Task DeleteAsync(Guid id);
    }
}
