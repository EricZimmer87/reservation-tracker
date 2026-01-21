using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace reservation_tracker.Models;

public partial class Guest
{
    [Key]
    public long GuestId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [Unicode(false)]
    public string Address { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string City { get; set; } = null!;

    [StringLength(2)]
    [Unicode(false)]
    public string State { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string Zipcode { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Unicode(false)]
    public string? Notes { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Company { get; set; }

    [InverseProperty("Guest")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
