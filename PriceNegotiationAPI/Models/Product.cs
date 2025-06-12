using System.ComponentModel.DataAnnotations;

namespace PriceNegotiationAPI.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        private Product() { }

        public Product(string name, decimal price, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name cannot be empty.", nameof(name));
            }
            if (price <= 0)
            {
                throw new ArgumentException("Product price must be greater than zero.", nameof(price));
            }

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
            Description = description;
        }
    }
}
