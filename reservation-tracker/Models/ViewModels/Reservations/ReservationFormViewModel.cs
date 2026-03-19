using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations;

public class ReservationFormViewModel : IValidatableObject
{
    public long? ReservationId { get; set; } // null for Create, set for Edit

    [Range(1, long.MaxValue, ErrorMessage = "Please select a guest.")]
    public long GuestId { get; set; }
    [Display(Name = "Reserved By")]
    public long? UserId { get; set; }

    [Required]
    public long RoomId { get; set; }

    [Required]
    public DateOnly CheckInDate { get; set; }

    [Required]
    public DateOnly CheckOutDate { get; set; }

    public int? NumberOfGuests { get; set; }
    public string? Notes { get; set; }

    [Required]
    public string Status { get; set; } = "";

    public string? CardLastFour { get; set; }

    // Return URLs
    public string? ReturnUrl { get; set; } // after creating reservation
    public string? GuestReturnUrl { get; set; } // to return back to reservation form

    // Double-booking:
    public bool ConfirmDoubleBooking { get; set; }
    public string? ConfirmAction { get; set; }
    // For displaying on confirm double-booking view
    public string? RoomNumber { get; set; }
    public string? GuestFullName { get; set; }
    public string? PhoneNumber { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "Check-out date must be later than check-in date.",
                new[] { nameof(CheckOutDate) }
            );
        }
    }
}
