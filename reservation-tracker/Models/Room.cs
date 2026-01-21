using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace reservation_tracker.Models;

[Index("RoomNumber", Name = "UQ__Rooms__AE10E07A4D92BE6F", IsUnique = true)]
public partial class Room
{
    [Key]
    public long RoomId { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string RoomNumber { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string RoomType { get; set; } = null!;

    [Unicode(false)]
    public string? Notes { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
