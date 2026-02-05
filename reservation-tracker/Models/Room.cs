using System;
using System.Collections.Generic;

namespace reservation_tracker.Models;

public partial class Room
{
    public long RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public string RoomType { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
