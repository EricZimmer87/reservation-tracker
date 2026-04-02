using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class ReservationIndexViewModel
    {
        public long ReservationId { get; set; }

        [Display(Name = "Date Reserved")]
        public DateTime DateReserved { get; set; }

        [Display(Name = "Last Updated On")]
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedByUserId { get; set; }

        [Display(Name = "Last Updated By")]
        public string? ModifiedByDisplayName { get; set; }

        [Display(Name = "Canceled On")]
        public DateTime? CanceledOn { get; set; }
        [Display(Name = "Canceled By")]
        public string? CanceledBy { get; set; }

        public long? GuestId { get; set; }

        [Display(Name = "Guest Name")]
        public string GuestFullName =>
            string.IsNullOrEmpty(GuestLastName)
            ? "-"
            : $"{GuestLastName}, {GuestFirstName}";
        public string? GuestLastName { get; set; }
        public string? GuestFirstName { get; set; }

        [Display(Name = "Phone Number")]
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

        [Display(Name = "Check-In Date")]
        public DateOnly CheckInDate { get; set; }

        [Display(Name = "Check-Out Date")]
        public DateOnly CheckOutDate { get; set; }

        [Display(Name = "Number of Guests")]
        public int? NumberOfGuests { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "";

        [Display(Name = "Card Last Four")]
        public string? CardLastFour { get; set; }

        public long RoomId { get; set; }

        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }

        [Display(Name = "Reserved By")]
        public string? ReservedByDisplayName { get; set; }
        public long? UserId { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
