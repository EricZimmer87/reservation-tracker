namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class ReservationIndexPageViewModel
    {
        public List<ReservationIndexViewModel> Reservations { get; set; } = new();

        public string? CurrentSort { get; set; }
        public string CurrentDir { get; set; }
    }
}
