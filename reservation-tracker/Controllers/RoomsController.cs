using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Data;
using reservation_tracker.Models;
using reservation_tracker.Models.ViewModels.Rooms;

namespace reservation_tracker.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ReservationTrackerContext _context;

        public RoomsController(ReservationTrackerContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(string sort, string dir)
        {
            dir = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase)
                ? "desc" : "asc";

            var rooms = _context.Rooms
                .Select(r => new RoomsIndexViewModel
                {
                    RoomId = r.RoomId,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    Notes = r.Notes
                });

            rooms = sort switch
            {
                "RoomId" => dir == "asc"
                ? rooms.OrderBy(r => r.RoomId)
                : rooms.OrderByDescending(r => r.RoomId),

                "RoomNumber" => dir == "asc"
                ? rooms.OrderBy(r => r.RoomNumber).ThenBy(r => r.RoomId)
                : rooms.OrderByDescending(r => r.RoomNumber).ThenBy(r => r.RoomId),

                "RoomType" => dir == "asc"
                ? rooms.OrderBy(r => r.RoomType).ThenBy(r => r.RoomId)
                : rooms.OrderByDescending(r => r.RoomType).ThenBy(r => r.RoomId),

                "Notes" => dir == "asc"
                ? rooms.OrderBy(r => r.Notes ?? "").ThenBy(r => r.RoomId)
                : rooms.OrderByDescending(r => r.Notes ?? "").ThenBy(r => r.RoomId),

                // Default sorting
                _ => rooms.OrderBy(r => r.RoomId)
            };

            var pageModel = new RoomsIndexPageViewModel
            {
                Rooms = await rooms.ToListAsync(),
                CurrentSort = sort,
                CurrentDir = dir
            };

            return View(pageModel);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public async Task<IActionResult> Create()
        {
            var model = new RoomFormViewModel
            {
                RoomTypeList = await _context.Rooms
                    .Select(r => r.RoomType)
                    .Where(rt => !string.IsNullOrWhiteSpace(rt))
                    .Distinct()
                    .OrderBy(rt => rt)
                    .ToListAsync()
            };

            return View(model);
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RoomTypeList = await _context.Rooms
                    .Select(r => r.RoomType)
                    .Where(rt => !string.IsNullOrWhiteSpace(rt))
                    .Distinct()
                    .OrderBy(rt => rt)
                    .ToListAsync();

                return View(model);
            }

            // Map view model to Entity
            var room = new Room
            {
                RoomNumber = model.RoomNumber,
                RoomType = model.RoomType,
                Notes = model.Notes
            };

            _context.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var model = new RoomFormViewModel
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                Notes = room.Notes,
                RoomTypeList = await _context.Rooms
                .Select(r => r.RoomType)
                .Where(rt => !string.IsNullOrWhiteSpace(rt))
                .Distinct()
                .OrderBy(rt => rt)
                .ToListAsync()
            };

            return View(model);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, RoomFormViewModel model)
        {
            if (id != model.RoomId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.RoomTypeList = await _context.Rooms
                    .Select(r => r.RoomType)
                    .Where(rt => !string.IsNullOrWhiteSpace(rt))
                    .Distinct()
                    .OrderBy(rt => rt)
                    .ToListAsync();

                return View(model);
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.RoomNumber = model.RoomNumber;
            room.RoomType = model.RoomType;
            room.Notes = model.Notes;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(long id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}
