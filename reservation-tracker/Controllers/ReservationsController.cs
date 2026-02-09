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
            ViewData["GuestId"] = new SelectList(_context.Guests
                .Select(g => new
                {
                    g.GuestId,
                    FullName = g.GuestId + " " + g.LastName + ", " + g.FirstName
                })
                .ToList(),
                "GuestId",
                "FullName");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,GuestId,UserId,RoomId,DateReserved,CheckInDate,CheckOutDate,NumberOfGuests,Notes,Status,CardLastFour")] Reservation reservation)
        {
            // Must have this to avoid Room field null error, as it cannot be null in model
            ModelState.Remove("Room");
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuestId"] = new SelectList(_context.Guests
              .Select(g => new
              {
                  g.GuestId,
                  FullName = g.GuestId + " " + g.LastName + ", " + g.FirstName
              })
              .ToList(),
              "GuestId",
              "FullName");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["GuestId"] = new SelectList(
                _context.Guests
                    .Select(g => new
                    {
                        g.GuestId,
                        FullName = g.GuestId + " " + g.LastName + ", " + g.FirstName
                    }),
                "GuestId",
                "FullName",
                reservation.GuestId
            );

            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ReservationId,GuestId,UserId,RoomId,DateReserved,CheckInDate,CheckOutDate,NumberOfGuests,Notes,Status,CardLastFour")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }
            // Must have this to avoid Room field null error, as it cannot be null in model
            ModelState.Remove("Room");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GuestId"] = new SelectList(
                _context.Guests
                    .Select(g => new
                    {
                        g.GuestId,
                        FullName = g.GuestId + " " + g.LastName + ", " + g.FirstName
                    }),
                "GuestId",
                "FullName",
                reservation.GuestId
            );
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", reservation.UserId);
            return View(reservation);
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
