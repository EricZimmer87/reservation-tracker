namespace reservation_tracker.Models.ViewModels.Reservations
{
    public class ConfirmDoubleBookingViewModel
    {
        public ReservationFormViewModel Reservation { get; set; } = new();
        public List<Reservation> OverlappingReservations { get; set; } = new();
    }
}
