using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Guests
{
    public class GuestFormViewModel
    {
        public long? GuestId { get; set; } // null for Create, set for Edit

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; } = "";

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; } = "";

        [Display(Name = "Name")]
        public string FullName =>
            string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName)
            ? "-"
            : $"{LastName}, {FirstName}";

        [Required]
        public string PhoneNumber { get; set; } = "";
        [Required]
        public string Address { get; set; } = "";
        [Required]
        public string City { get; set; } = "";
        [Required]
        public string State { get; set; } = "";
        [Required]
        public string Zipcode { get; set; } = "";

        public string? Email { get; set; }

        public string? Notes { get; set; }

        public string? Company { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
