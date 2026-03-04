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

        // Normalize data ranges to avoid reversing them
        static void Normalize(ref DateTime? from, ref DateTime? to)
        {
            if (from.HasValue && to.HasValue && from.Value > to.Value)
                (from, to) = (to, from);
        }
        static void Normalize(ref DateOnly? from, ref DateOnly? to)
        {
            if (from.HasValue && to.HasValue && from.Value > to.Value)
                (from, to) = (to, from);
        }

        // GET: Reservations
        public async Task<IActionResult> Index(
            string sort,
            string dir,
            string search,
            int pageSize = 10,
            int page = 1,
            string scope = "current",
            DateOnly? stayFrom = null, DateOnly? stayTo = null,
            DateTime? reservedFrom = null, DateTime? reservedTo = null)
        {
            dir = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase)
                ? "desc" : "asc";

            // Normalize data ranges
            Normalize(ref stayFrom, ref stayTo);
            Normalize(ref reservedFrom, ref reservedTo);

            // Set pageSize to default (10) if it is set to an unallowed value
            var allowedPageSizes = new[] { 10, 25, 50, 75, 100 };
            if (!allowedPageSizes.Contains(pageSize))
            {
                pageSize = 10;
            }

            // Ensure page is never lower than 1
            if (page < 1) page = 1;

            scope = (scope ?? "current").ToLowerInvariant();
            scope = scope is "past" or "current" or "all" ? scope : "current";

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

            // Determine if date ranges are being used
            var hasAnyRange =
                reservedFrom.HasValue || reservedTo.HasValue ||
                stayFrom.HasValue || stayTo.HasValue;

            // If any range is provided, override scope to all (so it searches everything)
            if (hasAnyRange) scope = "all";

            // Scope filter
            if (scope == "past")
                reservations = reservations.Where(r => r.CheckOutDate < today);
            else if (scope == "current")
                reservations = reservations.Where(r => r.CheckOutDate >= today);
            // "all" => no scope filter

            // DateReserved range
            if (reservedFrom.HasValue)
            {
                var from = reservedFrom.Value.Date;
                reservations = reservations.Where(r => r.DateReserved >= from);
            }
            if (reservedTo.HasValue)
            {
                var toExclusive = reservedTo.Value.Date.AddDays(1);
                reservations = reservations.Where(r => r.DateReserved < toExclusive);
            }

            // Stay overlap range
            if (stayFrom.HasValue || stayTo.HasValue)
            {
                var from = stayFrom ?? DateOnly.MinValue;
                var to = stayTo ?? DateOnly.MaxValue;

                reservations = reservations.Where(r =>
                    r.CheckInDate <= to &&
                    r.CheckOutDate >= from
                );
            }

            var projectedReservations = reservations.Select(r => new ReservationIndexViewModel
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
                RoomNumber = r.Room.RoomNumber,
                ReservedByDisplayName = r.User != null
                ? r.User.DisplayName : null
            });

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                projectedReservations = projectedReservations.Where(r =>
                    EF.Functions.Like(r.GuestLastName, $"%{search}%") ||
                    EF.Functions.Like(r.GuestFirstName, $"%{search}%") ||
                    EF.Functions.Like(r.RoomNumber, $"%{search}%") ||
                    EF.Functions.Like(r.ReservedByDisplayName, $"%{search}%") ||
                    EF.Functions.Like(r.GuestState, $"%{search}%") ||
                    EF.Functions.Like(r.GuestCity, $"%{search}%") ||
                    EF.Functions.Like(r.GuestZipcode, $"%{search}%") ||
                    (r.Notes != null && EF.Functions.Like(r.Notes, $"%{search}%")) ||
                    (r.CardLastFour != null && r.CardLastFour.Contains(search))
                );

                // Go back to page 1 when searching
                page = 1;
            }

            // Sort
            projectedReservations = sort switch
            {
                "DateReserved" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.DateReserved).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.DateReserved).ThenBy(r => r.ReservationId),

                "LastName" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.GuestLastName).ThenBy(r => r.GuestFirstName).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.GuestLastName).ThenBy(r => r.GuestFirstName).ThenBy(r => r.ReservationId),

                "Address" => dir == "asc"
                ? projectedReservations.OrderBy(r => r.GuestState).ThenBy(r => r.ReservationId)
                : projectedReservations.OrderByDescending(r => r.GuestState).ThenBy(r => r.ReservationId),

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

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            totalPages = Math.Max(1, totalPages);
            if (page > totalPages) page = totalPages;

            var items = await projectedReservations
                .Skip((page - 1) * pageSize) // filter out prev pages
                .Take(pageSize) // take only pageSize amount of res to display
                .ToListAsync();

            var pageModel = new ReservationIndexPageViewModel
            {
                Reservations = items,
                CurrentSort = sort,
                CurrentDir = dir,
                Search = search,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Scope = scope,
                StayFrom = stayFrom,
                StayTo = stayTo,
                ReservedFrom = reservedFrom,
                ReservedTo = reservedTo
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
        public IActionResult Create(long? roomId = null, long? guestId = null, DateOnly? checkInDate = null)
        {
            var start = checkInDate ?? DateOnly.FromDateTime(DateTime.Today);

            // Set default values for a new reservation
            var model = new ReservationFormViewModel
            {
                CheckInDate = start,
                CheckOutDate = start.AddDays(1),
                NumberOfGuests = 1,
                Status = "booked",
                RoomId = roomId ?? 0,
                GuestId = guestId ?? 0
            };

            PopulateSelectLists(
                guestId: model.GuestId == 0 ? (long?)null : model.GuestId,
                roomId: model.RoomId == 0 ? (long?)null : model.RoomId
            );

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
                PopulateSelectLists(
                    guestId: model.GuestId == 0 ? (long?)null : model.GuestId,
                    roomId: model.RoomId == 0 ? (long?)null : model.RoomId,
                    userId: model.UserId == 0 ? (long?)null : model.UserId
                );
                
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
                GuestId = entity.GuestId ?? 0,
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
