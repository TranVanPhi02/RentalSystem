using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public int ListingId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Apartment Listing { get; set; } = null!;

    public virtual Customer User { get; set; } = null!;
}
