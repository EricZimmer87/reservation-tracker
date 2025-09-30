using System;
using System.Collections.Generic;

namespace reservation_tracker_backend_dotnet.Models;

public partial class Guest
{
    public long GuestId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Email { get; set; }

    public string? Notes { get; set; }

    public string? Company { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
