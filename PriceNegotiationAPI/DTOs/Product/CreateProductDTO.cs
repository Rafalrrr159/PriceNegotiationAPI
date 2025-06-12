using System.ComponentModel.DataAnnotations;

namespace PriceNegotiationAPI.DTOs.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Product name must be between 3 and 250 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }
}
