using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Data;
using reservation_tracker.Models;
using reservation_tracker.Models.ViewModels.Reservations;

namespace reservation_tracker.Controllers
{
    public class ReservationsController(ReservationTrackerContext context) : Controller
    {
        private readonly ReservationTrackerContext _context = context;

        // Used to populate drop-down lists
        private void PopulateSelectLists(long? guestId = null, long? roomId = null, long? userId = null)
        {
            ViewData["GuestId"] = new SelectList(_context.Guests
                .Select(g => new
                {
                    g.GuestId,
                    FullName = g.LastName + ", " + g.FirstName
                })
                .ToList(),
                "GuestId",
                "FullName",
                guestId);

            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", roomId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", userId);
        }

        // GET: Reservations
        public async Task<IActionResult> Index(string sort, string dir, int page = 1, string scope = "current")
        {
            dir = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase)
                ? "desc" : "asc";

            // Fixed page size of 25
            const int pageSize = 25;
            // Ensure page is never lower than 1
            if (page < 1) page = 1;
            scope = string.Equals(scope, "past", StringComparison.OrdinalIgnoreCase) ? "past" : "current";

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Automatically change past reservations' status to "past"
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Reservations
                SET Status = {"past"}
                WHERE CheckOutDate < {today}
                  AND (Status = {"booked"} OR Status = {"checked_in"})
                  AND Status <> {"past"};
            ");

            var reservations = _context.Reservations
                .AsNoTracking(); // do not track in change tracker

            reservations = scope == "past"
                ? reservations.Where(r => r.CheckOutDate < today) // show past reservations
                : reservations.Where(r => r.CheckOutDate >= today); // show current reservations

            var projectedReservations = reservations.Select(r => new ReservationIndexViewModel
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
                RoomNumber = r.Room.RoomNumber,
                ReservedByDisplayName = r.User.DisplayName
            });

            projectedReservations = sort switch
            {
                "DateReserved" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.DateReserved).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.DateReserved).ThenBy(r => r.ReservationId),

                "LastName" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.GuestLastName).ThenBy(r => r.GuestFirstName).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.GuestLastName).ThenBy(r => r.GuestFirstName).ThenBy(r => r.ReservationId),

                "CheckInDate" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.CheckInDate).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.CheckInDate).ThenBy(r => r.ReservationId),

                "CheckOutDate" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.CheckOutDate).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.CheckOutDate).ThenBy(r => r.ReservationId),

                "NumberOfGuests" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.NumberOfGuests).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.NumberOfGuests).ThenBy(r => r.ReservationId),

                "Notes" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.Notes).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.Notes).ThenBy(r => r.ReservationId),

                "Status" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.Status).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.Status).ThenBy(r => r.ReservationId),

                "CardLastFour" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.CardLastFour).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.CardLastFour).ThenBy(r => r.ReservationId),

                "RoomNumber" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.RoomNumber).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.RoomNumber).ThenBy(r => r.ReservationId),

                "DisplayName" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.ReservedByDisplayName).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.ReservedByDisplayName).ThenBy(r => r.ReservationId),

                // Default sorting
                _ => projectedReservations.OrderBy(r => r.CheckInDate).ThenBy(r => r.ReservationId)
            };

            // Get total count to determine total amount of pages for displaying in view
            var totalCount = await projectedReservations.CountAsync();

            var items = await projectedReservations
                .Skip((page - 1) * pageSize) // filter out prev pages
                .Take(pageSize) // take only pageSize amount of res to display
                .ToListAsync();

            var pageModel = new ReservationIndexPageViewModel
            {
                Reservations = items,
                CurrentSort = sort,
                CurrentDir = dir,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Scope = scope
            };

            return View(pageModel);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            PopulateSelectLists();

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Set default values for a new reservation
            var model = new ReservationFormViewModel
            {
                CheckInDate = today,
                CheckOutDate = today.AddDays(1),
                NumberOfGuests = 1,
                Status = "booked"
            };

            return View(model);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateSelectLists(model.GuestId, model.RoomId, model.UserId);
                return View(model);
            }

            var entity = new Reservation
            {
                GuestId = model.GuestId,
                UserId = model.UserId,
                RoomId = model.RoomId,

                DateReserved = DateTime.Now,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,

                NumberOfGuests = model.NumberOfGuests,
                Notes = model.Notes,
                Status = model.Status,
                CardLastFour = model.CardLastFour
            };

            _context.Reservations.Add(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();

            var entity = await _context.Reservations.FindAsync(id);
            if (entity == null) return NotFound();

            // Populate fields with existing data
            var model = new ReservationFormViewModel
            {
                ReservationId = entity.ReservationId,
                GuestId = entity.GuestId,
                UserId = entity.UserId,
                RoomId = entity.RoomId,
                CheckInDate = entity.CheckInDate,
                CheckOutDate = entity.CheckOutDate,
                NumberOfGuests = entity.NumberOfGuests,
                Notes = entity.Notes,
                Status = entity.Status,
                CardLastFour = entity.CardLastFour
            };

            PopulateSelectLists(model.GuestId, model.RoomId, model.UserId);
            return View(model);
        }


        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ReservationFormViewModel model)
        {
            if (model.ReservationId == null || id != model.ReservationId.Value)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateSelectLists(model.GuestId, model.RoomId, model.UserId);
                return View(model);
            }

            var entity = await _context.Reservations.FindAsync(id);
            if (entity == null) return NotFound();

            entity.GuestId = model.GuestId;
            entity.UserId = model.UserId;
            entity.RoomId = model.RoomId;
            entity.CheckInDate = model.CheckInDate;
            entity.CheckOutDate = model.CheckOutDate;
            entity.NumberOfGuests = model.NumberOfGuests;
            entity.Notes = model.Notes;
            entity.Status = model.Status;
            entity.CardLastFour = model.CardLastFour;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(long id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
