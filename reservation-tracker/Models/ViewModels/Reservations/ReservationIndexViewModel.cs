using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class ReservationIndexViewModel
    {
        public long ReservationId { get; set; }
        public DateTime DateReserved { get; set; }

        public long? GuestId { get; set; }
        [Display(Name = "Guest")]
        public string GuestFullName =>
            string.IsNullOrEmpty(GuestLastName)
            ? "-"
            : $"{GuestLastName}, {GuestFirstName}";
        public string? GuestLastName { get; set; }
        public string? GuestFirstName { get; set; }
        public string? GuestPhoneNumber { get; set; }
        public string? GuestAddress { get; set; }
        public string? GuestCity { get; set; }
        public string? GuestState { get; set; }
        public string? GuestZipcode { get; set; }
        [Display(Name = "Address")]
        public string GuestFullAddress =>
            string.IsNullOrEmpty(GuestAddress)
            ? "-"
            : $"{GuestAddress}, {GuestCity}, {GuestState}, {GuestZipcode}";

        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "";
        public string? CardLastFour { get; set; }
        [Display(Name = "Reserved By")]
        public string? ReservedByDisplayName { get; set; }
        public long RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public long? UserId { get; set; }
        public string? User { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
