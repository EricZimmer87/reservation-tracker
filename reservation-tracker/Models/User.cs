using System;
using System.Collections.Generic;

namespace reservation_tracker.Models;

public partial class User
{
    public long UserId { get; set; }

    public string? GoogleId { get; set; }

    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Picture { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsBanned { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
