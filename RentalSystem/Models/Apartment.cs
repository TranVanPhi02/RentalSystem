using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Apartment
{
    public int ListingId { get; set; }

    public int LandlordId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public string Location { get; set; } = null!;

    public double Area { get; set; }

    public int Bedrooms { get; set; }

    public int Bathrooms { get; set; }

    public string? Images { get; set; }

    public DateOnly AvailableFrom { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CategoryId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Image> ImagesNavigation { get; set; } = new List<Image>();

    public virtual Customer Landlord { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
