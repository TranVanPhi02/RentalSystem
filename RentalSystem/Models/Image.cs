using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int? ApartmentId { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Apartment? Apartment { get; set; }
}
