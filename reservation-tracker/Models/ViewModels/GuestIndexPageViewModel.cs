namespace reservation_tracker.Models.ViewModels
{
    public class GuestIndexPageViewModel
    {
        public List<GuestIndexViewModel> Guests { get; set; } = new();

        public string? CurrentSort { get; set; }
        public string CurrentDir { get; set; }
    }
}
