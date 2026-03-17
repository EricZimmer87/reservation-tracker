using reservation_tracker.Models.ViewModels.Reservations;

namespace reservation_tracker.Models.ViewModels.Rooms
{
    public class DailyRoomRowViewModel
    {
        public long RoomId { get; set; }
        public string RoomNumber { get; set; } = "";
        public List<ReservationIndexViewModel> Reservations { get; set; } = new();

        // Don't count canceled reservation as occupying a room
        public bool IsOccupied =>
            Reservations.Any(r =>
                !string.Equals(r.Status, "Canceled", StringComparison.OrdinalIgnoreCase));
        public int ActiveReservationCount =>
            Reservations.Count(r =>
                !string.Equals(r.Status, "Canceled", StringComparison.OrdinalIgnoreCase));

        public bool IsDoubleBooked => ActiveReservationCount > 1;
    }
}
