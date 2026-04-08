using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Users
{
    public class UserIndexViewModel
    {
        [Display(Name = "User ID")]
        public long UserId { get; set; }

        [Display(Name = "Google ID")]
        public string? GoogleId { get; set; }

        public string Email { get; set; } = null!;

        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        public string? Picture { get; set; }

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Banned")]
        public bool IsBanned { get; set; }
    }
}
