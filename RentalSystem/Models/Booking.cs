using System;
using System.Collections.Generic;

namespace RentalSystem.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? ApartmentId { get; set; }

    public int? UserId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Apartment? Apartment { get; set; }

    public virtual User? User { get; set; }
}
