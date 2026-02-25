using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Data;
using reservation_tracker.Models;
using reservation_tracker.Models.ViewModels.Guests;

namespace reservation_tracker.Controllers
{
    public class GuestsController : Controller
    {
        private readonly ReservationTrackerContext _context;

        public GuestsController(ReservationTrackerContext context)
        {
            _context = context;
        }

        // GET: Guests
        public async Task<IActionResult> Index(string sort, string dir, string search, int page = 1)
        {
            dir = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase)
                ? "desc" : "asc";

            // Fixed page size of 25
            const int pageSize = 25;
            page = Math.Max(1, page);

            // Query guests
            var guests = _context.Guests
                .Select(g => new GuestIndexViewModel
                {
                    GuestId = g.GuestId,
                    FirstName = g.FirstName,
                    LastName = g.LastName,
                    PhoneNumber = g.PhoneNumber,
                    Address = g.Address,
                    City = g.City,
                    State = g.State,
                    Zipcode = g.Zipcode,
                    Email = g.Email,           
                    Notes = g.Notes,
                    Company = g.Company
                });

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                guests = guests.Where(g =>
                    g.FirstName.ToLower().Contains(search) ||
                    g.LastName.ToLower().Contains(search) ||
                    g.PhoneNumber.ToLower().Contains(search) ||
                    g.Address.ToLower().Contains(search) ||
                    g.City.ToLower().Contains(search) ||
                    g.State.ToLower().Contains(search) ||
                    g.Zipcode.ToLower().Contains(search) ||
                    (g.Email != null && g.Email.ToLower().Contains(search)) ||
                    (g.Notes != null && g.Notes.ToLower().Contains(search)) ||
                    (g.Company != null && g.Company.ToLower().Contains(search))
                );

                // Go back to page 1 when searching
                page = 1;
            }

            // Sort
            guests = sort switch
            {
                "LastName" => dir == "asc"
                ? guests.OrderBy(g => g.LastName).ThenBy(g => g.FirstName)
                : guests.OrderByDescending(g => g.LastName).ThenBy(g => g.FirstName),

                "PhoneNumber" => dir == "asc"
                ? guests.OrderBy(g => g.PhoneNumber)
                : guests.OrderByDescending(g => g.PhoneNumber),

                "Address" => dir == "asc"
                ? guests.OrderBy(g => g.Address)
                : guests.OrderByDescending(g => g.Address),

                "City" => dir == "asc"
                ? guests.OrderBy(g => g.City)
                : guests.OrderByDescending(g => g.City),

                "State" => dir == "asc"
                ? guests.OrderBy(g => g.State)
                : guests.OrderByDescending(g => g.State),

                "Zipcode" => dir == "asc"
                ? guests.OrderBy(g => g.Zipcode)
                : guests.OrderByDescending(g => g.Zipcode),

                "Email" => dir == "asc"
                ? guests.OrderBy(g => g.Email)
                : guests.OrderByDescending(g => g.Email),

                "Notes" => dir == "asc"
                ? guests.OrderBy(g => g.Notes)
                : guests.OrderByDescending(g => g.Notes),

                "Company" => dir == "asc"
                ? guests.OrderBy(g => g.Company)
                : guests.OrderByDescending(g => g.Company),

                // Default sorting
                _ => guests.OrderBy(g => g.LastName)
            };

            // Get total count of guests for pagination
            var totalCount = await guests.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var pageModel = new GuestIndexPageViewModel
            {
                Guests = await guests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
                CurrentSort = sort,
                CurrentDir = dir,
                Search = search,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return View(pageModel);
        }

        // GET: Guests/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.GuestId == id);
            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // GET: Guests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Guests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GuestId,FirstName,LastName,PhoneNumber,Address,City,State,Zipcode,Email,Notes,Company")] Guest guest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        // GET: Guests/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests.FindAsync(id);
            if (guest == null)
            {
                return NotFound();
            }
            return View(guest);
        }

        // POST: Guests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("GuestId,FirstName,LastName,PhoneNumber,Address,City,State,Zipcode,Email,Notes,Company")] Guest guest)
        {
            if (id != guest.GuestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestExists(guest.GuestId))
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
            return View(guest);
        }

        // GET: Guests/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var guest = await _context.Guests
                .FirstOrDefaultAsync(m => m.GuestId == id);
            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // POST: Guests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var guest = await _context.Guests.FindAsync(id);
            if (guest != null)
            {
                _context.Guests.Remove(guest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GuestExists(long id)
        {
            return _context.Guests.Any(e => e.GuestId == id);
        }
    }
}
