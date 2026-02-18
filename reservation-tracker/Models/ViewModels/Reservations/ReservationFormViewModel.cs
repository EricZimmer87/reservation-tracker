using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations;

public class ReservationFormViewModel : IValidatableObject
{
    public long? ReservationId { get; set; } // null for Create, set for Edit

    public long? GuestId { get; set; }
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
