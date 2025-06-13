using AutoMapper;
using PriceNegotiationAPI.DTOs.Negotiation;
using PriceNegotiationAPI.Enums;
using PriceNegotiationAPI.Exceptions;
using PriceNegotiationAPI.Interfaces;
using PriceNegotiationAPI.Models;

namespace PriceNegotiationAPI.Services
{
    public class NegotiationService : INegotiationService
    {
        private readonly INegotiationRepository _negotiationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        private const int MAX_ATTEMPTS = 3;
        private const int REJECTION_EXPIRY_DAYS = 7;

        public NegotiationService(INegotiationRepository negotiationRepository, IProductRepository productRepository, IMapper mapper)
        {
            _negotiationRepository = negotiationRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<NegotiationDto?> GetNegotiationByIdAsync(Guid negotiationId)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(negotiationId);
            if (negotiation == null) return null;

            await CheckAndExpireNegotiation(negotiation);

            return _mapper.Map<NegotiationDto>(negotiation);
        }

        public async Task<List<NegotiationDto>> GetAllNegotiationsForProductAsync(Guid productId)
        {
            var negotiations = await _negotiationRepository.GetAllByProductIdAsync(productId);

            foreach (var neg in negotiations)
            {
                await CheckAndExpireNegotiation(neg);
            }

            return _mapper.Map<List<NegotiationDto>>(negotiations);
        }

        public async Task<List<NegotiationDto>> GetActiveNegotiationsAsync()
        {
            var allNegotiations = await _negotiationRepository.GetAllByProductIdAsync(Guid.Empty);

            var activeDtos = new List<NegotiationDto>();
            foreach (var neg in allNegotiations)
            {
                await CheckAndExpireNegotiation(neg);
                if (neg.Status == NegotiationStatus.Proposed || neg.Status == NegotiationStatus.Rejected)
                {
                    activeDtos.Add(_mapper.Map<NegotiationDto>(neg));
                }
            }
            return activeDtos;
        }

        public async Task<NegotiationDto> ProposePriceAsync(CreateNegotiationDto createDto)
        {
            var product = await _productRepository.GetByIdAsync(createDto.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {createDto.ProductId} not found.");
            }
            if (createDto.ProposedPrice <= 0)
            {
                throw new ArgumentException("Proposed price must be greater than zero.", nameof(createDto.ProposedPrice));
            }
            if (createDto.ClientId == Guid.Empty)
            {
                throw new ArgumentException("Client ID must be provided.", nameof(createDto.ClientId));
            }

            var allClientProductNegotiations = await _negotiationRepository.GetNegotiationsByClientIdAndProductIdAsync(createDto.ClientId, createDto.ProductId);

            foreach (var neg in allClientProductNegotiations)
            {
                await CheckAndExpireNegotiation(neg);
            }

            bool clientExhaustedAttemptsForProduct = allClientProductNegotiations
                .Any(n => n.Status == NegotiationStatus.Expired);

            if (clientExhaustedAttemptsForProduct)
            {
                throw new InvalidOperationException($"Client has exhausted all negotiation attempts for product {createDto.ProductId}. No further proposals allowed.");
            }

            var activeNegotiation = allClientProductNegotiations
                .FirstOrDefault(n => n.Status == NegotiationStatus.Proposed || n.Status == NegotiationStatus.Rejected || n.Status == NegotiationStatus.Accepted);

            if (activeNegotiation != null)
            {
                if (activeNegotiation.Status == NegotiationStatus.Rejected)
                {
                    activeNegotiation.ProposedPrice = createDto.ProposedPrice;
                    activeNegotiation.Status = NegotiationStatus.Proposed;
                    activeNegotiation.AttemptsCount++;
                    activeNegotiation.ProposedDate = DateTime.UtcNow;
                    activeNegotiation.LastRejectionDate = null;
                    activeNegotiation.LastModifiedDate = DateTime.UtcNow;

                    await _negotiationRepository.UpdateAsync(activeNegotiation);
                    return _mapper.Map<NegotiationDto>(activeNegotiation);
                }
                else if (activeNegotiation.Status == NegotiationStatus.Proposed)
                {
                    throw new InvalidOperationException($"Negotiation {activeNegotiation.Id} is currently in 'Proposed' status for product {createDto.ProductId}. Please await a decision from the store employee before making a new proposal.");
                }
                else if (activeNegotiation.Status == NegotiationStatus.Accepted)
                {
                    throw new InvalidOperationException($"Negotiation {activeNegotiation.Id} for product {createDto.ProductId} has already been accepted with price {activeNegotiation.ProposedPrice}. You cannot propose a new price for this product through this negotiation.");
                }
            }
            var newNegotiation = new Negotiation(createDto.ProductId, createDto.ProposedPrice, createDto.ClientId);
            await _negotiationRepository.AddAsync(newNegotiation);
            return _mapper.Map<NegotiationDto>(newNegotiation);
        }

        public async Task<NegotiationDto> AcceptNegotiationAsync(Guid negotiationId)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(negotiationId);
            if (negotiation == null)
            {
                throw new NotFoundException($"Negotiation with ID {negotiationId} not found.");
            }

            await CheckAndExpireNegotiation(negotiation);
            if (negotiation.Status != NegotiationStatus.Proposed)
            {
                throw new InvalidOperationException($"Negotiation {negotiationId} cannot be accepted from status {negotiation.Status}. Only 'Proposed' negotiations can be accepted.");
            }

            negotiation.Status = NegotiationStatus.Accepted;
            negotiation.LastModifiedDate = DateTime.UtcNow;
            await _negotiationRepository.UpdateAsync(negotiation);
            return _mapper.Map<NegotiationDto>(negotiation);
        }

        public async Task<NegotiationDto> RejectNegotiationAsync(Guid negotiationId)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(negotiationId);
            if (negotiation == null)
            {
                throw new NotFoundException($"Negotiation with ID {negotiationId} not found.");
            }

            await CheckAndExpireNegotiation(negotiation);
            if (negotiation.Status != NegotiationStatus.Proposed)
            {
                throw new InvalidOperationException($"Negotiation {negotiationId} cannot be rejected from status {negotiation.Status}. Only 'Proposed' negotiations can be rejected.");
            }

            negotiation.Status = NegotiationStatus.Rejected;
            negotiation.LastRejectionDate = DateTime.UtcNow;
            negotiation.LastModifiedDate = DateTime.UtcNow;

            if (negotiation.AttemptsCount >= MAX_ATTEMPTS)
            {
                negotiation.Status = NegotiationStatus.Expired;
            }

            await _negotiationRepository.UpdateAsync(negotiation);
            return _mapper.Map<NegotiationDto>(negotiation);
        }

        public async Task<NegotiationDto> CancelNegotiationAsync(Guid negotiationId)
        {
            var negotiation = await _negotiationRepository.GetByIdAsync(negotiationId);
            if (negotiation == null)
            {
                throw new NotFoundException($"Negotiation with ID {negotiationId} not found.");
            }

            await CheckAndExpireNegotiation(negotiation);
            if (negotiation.Status != NegotiationStatus.Proposed && negotiation.Status != NegotiationStatus.Rejected)
            {
                throw new InvalidOperationException($"Negotiation {negotiationId} cannot be canceled from status {negotiation.Status}. Only 'Proposed' or 'Rejected' negotiations can be canceled.");
            }

            negotiation.Status = NegotiationStatus.Canceled;
            negotiation.LastModifiedDate = DateTime.UtcNow;
            await _negotiationRepository.UpdateAsync(negotiation);
            return _mapper.Map<NegotiationDto>(negotiation);
        }

        private async Task CheckAndExpireNegotiation(Negotiation negotiation)
        {
            if (negotiation.Status == NegotiationStatus.Rejected &&
                negotiation.LastRejectionDate.HasValue &&
                (DateTime.UtcNow - negotiation.LastRejectionDate.Value).TotalDays > REJECTION_EXPIRY_DAYS &&
                negotiation.Status != NegotiationStatus.Expired)
            {
                negotiation.Status = NegotiationStatus.Expired;
                negotiation.LastModifiedDate = DateTime.UtcNow;
                await _negotiationRepository.UpdateAsync(negotiation);
            }
        }
    }
}
