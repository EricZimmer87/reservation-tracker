using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace reservation_tracker.Models;

public partial class Reservation
{
    [Key]
    public long ReservationId { get; set; }

    public long? GuestId { get; set; }

    public long? UserId { get; set; }

    public long RoomId { get; set; }

    public DateTime DateReserved { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public int? NumberOfGuests { get; set; }

    [Unicode(false)]
    public string? Notes { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [StringLength(4)]
    [Unicode(false)]
    public string? CardLastFour { get; set; }

    [ForeignKey("GuestId")]
    [InverseProperty("Reservations")]
    public virtual Guest? Guest { get; set; }

    [ForeignKey("RoomId")]
    [InverseProperty("Reservations")]
    public virtual Room Room { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reservations")]
    public virtual User? User { get; set; }
}
