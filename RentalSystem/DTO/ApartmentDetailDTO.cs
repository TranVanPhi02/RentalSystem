using RentalSystem.Models;

namespace RentalSystem.DTO
{
    public class ApartmentDetailDTO
    {
        public Apartment Apartment { get; set; }
        public List<Apartment> RelatedApartments { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
