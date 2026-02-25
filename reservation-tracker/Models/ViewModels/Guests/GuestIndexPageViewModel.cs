namespace reservation_tracker.Models.ViewModels.Guests
{
    public class GuestIndexPageViewModel
    {
        public List<GuestIndexViewModel> Guests { get; set; } = new();

        public string? CurrentSort { get; set; }
        public string CurrentDir { get; set; }
        public string Search { get; set; }


        // Pagination properties
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrev => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
