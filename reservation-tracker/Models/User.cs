using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace reservation_tracker.Models;

[Index("GoogleId", Name = "UQ__Users__A6FBF2FBC690DDEF", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D10534483ACB35", IsUnique = true)]
public partial class User
{
    [Key]
    public long UserId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? GoogleId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? DisplayName { get; set; }

    [Unicode(false)]
    public string? Picture { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsBanned { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
