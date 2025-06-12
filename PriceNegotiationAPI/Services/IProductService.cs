using PriceNegotiationAPI.DTOs.Product;

namespace PriceNegotiationAPI.Services
{
    public interface IProductService
    {
        Task<Guid> CreateProductAsync(CreateProductDto createDto);
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
}
