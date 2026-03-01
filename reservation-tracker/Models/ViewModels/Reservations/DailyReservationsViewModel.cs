using reservation_tracker.Models.ViewModels.Rooms;
using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class DailyReservationsViewModel
    {
            public List<DailyRoomRowViewModel> Rooms { get; set; } = new();
            public DateOnly SelectedDay { get; set; }
    }
}
