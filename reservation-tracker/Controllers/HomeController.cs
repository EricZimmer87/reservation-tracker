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

        public async Task<IActionResult> Index(DateOnly? selectedDay)
        {
            var day = selectedDay ?? DateOnly.FromDateTime(DateTime.Today);

            // Get the rooms
            var roomNumbers = await _context.Rooms
                .AsNoTracking()
                .OrderBy(r => r.RoomNumber)
                .Select(r => r.RoomNumber)
                .ToListAsync();

            // Get today's reservations
            var todaysReservations = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.CheckInDate <= day && r.CheckOutDate > day)
                .Select(r => new ReservationIndexViewModel
                {
                    ReservationId = r.ReservationId,
                    DateReserved = r.DateReserved,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,

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
                    RoomNumber = r.Room.RoomNumber,
                    ReservedByDisplayName = r.User.DisplayName
                })
                .ToListAsync();

            var byRoom = todaysReservations.ToDictionary(r => r.RoomNumber);

            var rows = roomNumbers.Select(rn => new DailyRoomRowViewModel
            {
                RoomNumber = rn,
                Reservation = byRoom.TryGetValue(rn, out var res) ? res : null
            }).ToList();

            var vm = new DailyReservationsViewModel
            {
                SelectedDay = day,
                Rooms = rows
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
