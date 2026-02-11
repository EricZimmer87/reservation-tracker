using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models;

public class ReservationFormViewModel : IValidatableObject
{
    public long? ReservationId { get; set; } // null for Create, set for Edit

    public long? GuestId { get; set; }
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
        // If either date didn't bind, don't do the comparison yet.
        // (Let the [Required] / binder errors handle it.)
        if (CheckInDate == default || CheckOutDate == default)
            yield break;

        if (CheckOutDate <= CheckInDate)
        {
            yield return new ValidationResult(
                "Check-out date must be later than check-in date.",
                new[] { nameof(CheckOutDate) }
            );
        }
    }
}
