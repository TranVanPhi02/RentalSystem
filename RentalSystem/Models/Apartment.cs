using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Apartment
{
    public int ApartmentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal Area { get; set; }

    public string Location { get; set; } = null!;

    public string? Address { get; set; }

    public int NumberOfBedrooms { get; set; }

    public int NumberOfBathrooms { get; set; }

    public string? Amenities { get; set; }

    public string? Images { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Image> ImagesNavigation { get; set; } = new List<Image>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? User { get; set; }
}
