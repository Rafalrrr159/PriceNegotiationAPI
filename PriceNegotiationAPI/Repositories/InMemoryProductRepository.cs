using PriceNegotiationAPI.Interfaces;
using PriceNegotiationAPI.Models;

namespace PriceNegotiationAPI.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private static readonly List<Product> _products = new List<Product>();

        public Task AddAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Product>>(_products.AsEnumerable());
        }
    }
}
