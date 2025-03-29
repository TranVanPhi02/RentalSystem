using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int TenantId { get; set; }

    public int ListingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Apartment Listing { get; set; } = null!;

    public virtual Customer Tenant { get; set; } = null!;
}
