using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Users
{
    public class UserDetailsViewModel
    {
        public long UserId { get; set; }

        [Display(Name = "Google ID")]
        public string? GoogleId { get; set; }

        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [Display(Name = "Picture")]
        public string? Picture { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Banned")]
        public bool IsBanned { get; set; }
    }
}
