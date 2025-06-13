using PriceNegotiationAPI.DTOs.Negotiation;

namespace PriceNegotiationAPI.Services
{
    public interface INegotiationService
    {
        Task<NegotiationDto?> GetNegotiationByIdAsync(Guid negotiationId);
        Task<List<NegotiationDto>> GetAllNegotiationsForProductAsync(Guid productId);
        Task<List<NegotiationDto>> GetActiveNegotiationsAsync();
        Task<NegotiationDto> ProposePriceAsync(CreateNegotiationDto createDto);
        Task<NegotiationDto> AcceptNegotiationAsync(Guid negotiationId);
        Task<NegotiationDto> RejectNegotiationAsync(Guid negotiationId);
        Task<NegotiationDto> CancelNegotiationAsync(Guid negotiationId);
    }
}
