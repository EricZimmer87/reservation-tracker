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

        public async Task<IActionResult> Index(DateOnly? selectedDay, string? sort, string? dir)
        {
            dir = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
            var day = selectedDay ?? DateOnly.FromDateTime(DateTime.Today);

            var baseQuery = _context.Rooms
                .AsNoTracking()
                .Select(room => new DailyRoomRowViewModel
                {
                    RoomNumber = room.RoomNumber,
                    Reservation = room.Reservations
                        .Where(r => r.CheckInDate <= day && r.CheckOutDate > day)
                        .Select(r => new ReservationIndexViewModel
                        {
                            ReservationId = r.ReservationId,
                            DateReserved = r.DateReserved,
                            CheckInDate = r.CheckInDate,
                            CheckOutDate = r.CheckOutDate,
                            GuestLastName = r.Guest.LastName,
                            GuestFirstName = r.Guest.FirstName,
                            NumberOfGuests = r.NumberOfGuests,
                            Notes = r.Notes,
                            Status = r.Status,
                            CardLastFour = r.CardLastFour,
                            RoomNumber = room.RoomNumber,
                            ReservedByDisplayName = r.User.DisplayName
                        })
                        .FirstOrDefault()
                });

            // Helpers for null-safe sorting:
            // - first key: whether reservation is null (false=has reservation, true=vacant)
            //   This pushes vacant rooms to the bottom. Flip the boolean if you want vacant first.
            IOrderedQueryable<DailyRoomRowViewModel> projected = sort switch
            {
                "RoomNumber" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.RoomNumber)
                    : baseQuery.OrderByDescending(x => x.RoomNumber),

                "DateReserved" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.DateReserved)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.DateReserved)
                              .ThenBy(x => x.RoomNumber),

                "LastName" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.GuestLastName)
                              .ThenBy(x => x.Reservation!.GuestFirstName)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.GuestLastName)
                              .ThenBy(x => x.Reservation!.GuestFirstName)
                              .ThenBy(x => x.RoomNumber),

                "CheckInDate" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.CheckInDate)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.CheckInDate)
                              .ThenBy(x => x.RoomNumber),

                "CheckOutDate" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.CheckOutDate)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.CheckOutDate)
                              .ThenBy(x => x.RoomNumber),

                "NumberOfGuests" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.NumberOfGuests) // int? ok
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.NumberOfGuests)
                              .ThenBy(x => x.RoomNumber),

                "Notes" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.Notes)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.Notes)
                              .ThenBy(x => x.RoomNumber),

                "Status" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.Status)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.Status)
                              .ThenBy(x => x.RoomNumber),

                "CardLastFour" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.CardLastFour)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.CardLastFour)
                              .ThenBy(x => x.RoomNumber),

                "DisplayName" => dir == "asc"
                    ? baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenBy(x => x.Reservation!.ReservedByDisplayName)
                              .ThenBy(x => x.RoomNumber)
                    : baseQuery.OrderBy(x => x.Reservation == null)
                              .ThenByDescending(x => x.Reservation!.ReservedByDisplayName)
                              .ThenBy(x => x.RoomNumber),

                _ => baseQuery.OrderBy(x => x.RoomNumber)
            };

            var items = new DailyReservationsViewModel
            {
                SelectedDay = day,
                CurrentSort = sort,
                CurrentDir = dir,
                Rooms = await projected.ToListAsync()
            };

            return View(items);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
