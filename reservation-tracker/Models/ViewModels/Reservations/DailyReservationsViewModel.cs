using reservation_tracker.Models.ViewModels.Rooms;

namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class DailyReservationsViewModel
    {
        public List<DailyRoomRowViewModel> Rooms { get; set; } = new();
        public DateOnly SelectedDay { get; set; }
        public bool ShowCanceled { get; set; }
    }
}
