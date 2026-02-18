using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class ReservationIndexViewModel
    {
        public long ReservationId { get; set; }
        public DateTime DateReserved { get; set; }
        [Display(Name = "Guest")]
        public string GuestFullName =>
            string.IsNullOrEmpty(GuestLastName)
            ? "-"
            : $"{GuestLastName}, {GuestFirstName}";
        public string GuestLastName { get; set; }
        public string GuestFirstName { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "";
        public string? CardLastFour { get; set; }
        [Display(Name = "Reserved By")]
        public string ReservedByDisplayName { get; set; }
        public string RoomNumber { get; set; }

    }
}
