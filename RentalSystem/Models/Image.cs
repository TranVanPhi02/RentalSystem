using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int ListingId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual Apartment Listing { get; set; } = null!;
}
