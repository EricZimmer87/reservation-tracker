using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Users
{
    public class UserCreateViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Banned")]
        public bool IsBanned { get; set; }
    }
}
