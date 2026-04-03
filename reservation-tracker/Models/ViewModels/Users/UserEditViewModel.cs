using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Users
{
    public class UserEditViewModel
    {
        public long UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Banned")]
        public bool IsBanned { get; set; }
    }
}
