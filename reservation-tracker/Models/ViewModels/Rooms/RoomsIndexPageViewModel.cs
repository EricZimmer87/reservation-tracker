namespace reservation_tracker.Models.ViewModels.Rooms
{
    public class RoomsIndexPageViewModel
    {
        public List<RoomsIndexViewModel> Rooms { get; set; } = new();

        public string? CurrentSort { get; set; }
        public string? CurrentDir { get; set; }
    }
}
