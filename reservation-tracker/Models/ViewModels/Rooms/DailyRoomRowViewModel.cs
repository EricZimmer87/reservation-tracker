using reservation_tracker.Models.ViewModels.Reservations;

namespace reservation_tracker.Models.ViewModels.Rooms
{
    public class DailyRoomRowViewModel
    {
        public long RoomId { get; set; }
        public string RoomNumber { get; set; } = "";
        public ReservationIndexViewModel? Reservation { get; set; }  // null => vacant
    }
}
