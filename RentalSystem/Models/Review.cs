using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int TenantId { get; set; }

    public int ListingId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? AdminReply { get; set; }

    public DateTime? AdminReplyAt { get; set; }

    public virtual Apartment Listing { get; set; } = null!;

    public virtual Customer Tenant { get; set; } = null!;
}
