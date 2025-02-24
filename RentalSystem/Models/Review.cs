using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? ApartmentId { get; set; }

    public int? UserId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Apartment? Apartment { get; set; }

    public virtual User? User { get; set; }
}
