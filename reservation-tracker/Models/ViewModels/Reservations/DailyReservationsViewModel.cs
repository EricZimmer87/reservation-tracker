using reservation_tracker.Models.ViewModels.Rooms;
using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class DailyReservationsViewModel
    {
            public List<DailyRoomRowViewModel> Rooms { get; set; } = new();

            public string? CurrentSort { get; set; }
            public string CurrentDir { get; set; } = "asc";
            public DateOnly SelectedDay { get; set; }
    }
}
