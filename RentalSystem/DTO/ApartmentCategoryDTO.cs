using RentalSystem.Models;

namespace RentalSystem.DTO
{
    public class ApartmentCategoryDTO
    {
        public List<Apartment> Apartments { get; set; }
        public List<Category> Categories { get; set; }
    }
}
