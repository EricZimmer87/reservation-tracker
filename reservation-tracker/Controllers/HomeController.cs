using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Data;
using reservation_tracker.Models;
using reservation_tracker.Models.ViewModels.Reservations;
using reservation_tracker.Models.ViewModels.Rooms;
using System.Diagnostics;

namespace reservation_tracker.Controllers
{
    public class HomeController(ReservationTrackerContext context) : Controller
    {
        private readonly ReservationTrackerContext _context = context;

        // Displays the rooms with reservations (if any) for the day
        public async Task<IActionResult> Index(DateOnly? selectedDay, bool showCanceled = false)
        {
            var day = selectedDay ?? DateOnly.FromDateTime(DateTime.Today);

            // Get the rooms sorted by room number
            var rooms = await _context.Rooms
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .Select(r => new { r.RoomId, r.RoomNumber })
                .ToListAsync();

            // Get the day's reservations
            var todaysReservations = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.CheckInDate <= day && r.CheckOutDate > day)
                .Select(r => new ReservationIndexViewModel
                {
                    ReservationId = r.ReservationId,
                    DateReserved = r.DateReserved,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,

                    GuestId = r.GuestId,
                    GuestLastName = r.Guest != null ? r.Guest.LastName : null,
                    GuestFirstName = r.Guest != null ? r.Guest.FirstName : null,
                    GuestPhoneNumber = r.Guest != null ? r.Guest.PhoneNumber : null,
                    GuestAddress = r.Guest != null ? r.Guest.Address : null,
                    GuestCity = r.Guest != null ? r.Guest.City : null,
                    GuestState = r.Guest != null ? r.Guest.State : null,
                    GuestZipcode = r.Guest != null ? r.Guest.Zipcode : null,

                    NumberOfGuests = r.NumberOfGuests,
                    Notes = r.Notes,
                    Status = r.Status,
                    CardLastFour = r.CardLastFour,
                    RoomId = r.Room.RoomId,
                    RoomNumber = r.Room.RoomNumber,
                    ReservedByDisplayName = r.User != null
                    ? r.User.DisplayName : null
                })
                .ToListAsync();

            // Group reservations by room into a dictionary - roomId:reservations
            var byRoom = todaysReservations
                .GroupBy(r => r.RoomId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // For each room, build a row with the reservations we want to display
            var rows = rooms.Select(room =>
            {
                var reservations = byRoom.TryGetValue(room.RoomId, out var roomReservations)
                    ? roomReservations
                    : new List<ReservationIndexViewModel>();

                // If showCanceled is true, show canceled reservations, else filter them out
                var visibleReservations = showCanceled
                    ? reservations
                    : reservations
                        .Where(r => !string.Equals(r.Status, "Canceled", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                // Create the row
                return new DailyRoomRowViewModel
                {
                    RoomId = room.RoomId,
                    RoomNumber = room.RoomNumber,
                    Reservations = visibleReservations
                };
            }).ToList();

            // Set the model
            var vm = new DailyReservationsViewModel
            {
                SelectedDay = day,
                Rooms = rows,
                ShowCanceled = showCanceled
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
