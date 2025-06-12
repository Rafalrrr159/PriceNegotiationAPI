using AutoMapper;
using PriceNegotiationAPI.DTOs.Product;
using PriceNegotiationAPI.Exceptions;
using PriceNegotiationAPI.Interfaces;
using PriceNegotiationAPI.Models;

namespace PriceNegotiationAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Guid> CreateProductAsync(CreateProductDto createDto)
        {
            var product = _mapper.Map<Product>(createDto);

            await _productRepository.AddAsync(product);

            return product.Id;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                throw new NotFoundException(nameof(Product), id);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(products);
            return productDtos;
        }
    }
}
