namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class ReservationIndexPageViewModel
    {
        public List<ReservationIndexViewModel> Reservations { get; set; } = new();

        public string? CurrentSort { get; set; }
        public string CurrentDir { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrev => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
