using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels
{
    public class GuestIndexViewModel
    {
        public long GuestId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        [Display(Name = "Name")]
        public string FullName =>
            string.IsNullOrEmpty(LastName)
            ? "-"
            : $"{LastName}, {FirstName}";

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zipcode { get; set; }

        public string? Email { get; set; }

        public string? Notes { get; set; }

        public string? Company { get; set; }
    }
}
