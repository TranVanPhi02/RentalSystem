using System.ComponentModel.DataAnnotations;

namespace RentalSystem.DTO
{
    public class ApartmentDTO
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Area must be greater than 0.")]
        public double Area { get; set; }

        [Required]
        public int Bedrooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public DateOnly AvailableFrom { get; set; }

        public int? CategoryId { get; set; } // Không bắt buộc
        public string? Status { get; set; } // Không bắt buộc
        public string? Images { get; set; } // Không bắt buộc
    }
}
