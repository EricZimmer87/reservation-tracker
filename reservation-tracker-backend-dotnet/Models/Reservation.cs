using System;
using System.Collections.Generic;

namespace reservation_tracker_backend_dotnet.Models;

public partial class Reservation
{
    public long ReservationId { get; set; }

    public long? GuestId { get; set; }

    public long? UserId { get; set; }

    public long RoomId { get; set; }

    public DateTime? DateReserved { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int? NumberOfGuests { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public string? CardLastFour { get; set; }

    public virtual Guest? Guest { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User? User { get; set; }
}
