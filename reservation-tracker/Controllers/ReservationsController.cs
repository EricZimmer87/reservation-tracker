using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Data;
using reservation_tracker.Models;

namespace reservation_tracker.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ReservationTrackerContext _context;

        public ReservationsController(ReservationTrackerContext context)
        {
            _context = context;
        }

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
        public async Task<IActionResult> Index(string sort, string dir)
        {
            dir = dir?.ToLower() == "desc" ? "desc" : "asc";

            var reservations = _context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .Include(r => r.User)
                .OrderBy(r => r.CheckInDate);

            reservations = sort switch
            {
                "DateReserved" => dir == "asc"
                ? reservations.OrderBy(r => r.DateReserved)
                : reservations.OrderByDescending(r => r.DateReserved),

                "CheckInDate" => dir == "asc"
                ? reservations.OrderBy(r => r.CheckInDate)
                : reservations.OrderByDescending(r => r.CheckInDate),

                "CheckOutDate" => dir == "asc"
                ? reservations.OrderBy(r => r.CheckOutDate)
                : reservations.OrderByDescending(r => r.CheckOutDate),

                "LastName" => dir == "asc"
                ? reservations.OrderBy(r => r.Guest.LastName)
                : reservations.OrderByDescending(r => r.Guest.LastName),

                "RoomNumber" => dir == "asc"
                ? reservations.OrderBy(r => r.Room.RoomNumber)
                : reservations.OrderByDescending(r => r.Room.RoomNumber),

                // Default sorting
                _ => reservations.OrderBy(r => r.CheckInDate)
            };

            ViewData["Sort"] = sort;
            ViewData["Dir"] = dir;

            return View(await reservations.ToListAsync());
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
