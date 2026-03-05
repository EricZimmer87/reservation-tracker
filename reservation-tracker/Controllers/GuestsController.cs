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

        // Search for Guests
        [HttpGet]
        public IActionResult Search(string? q, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var results = Enumerable.Empty<Guest>();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                var like = $"%{q}%";

                results = _context.Guests
                    .Where(g =>
                        EF.Functions.Like(g.FirstName ?? "", like) ||
                        EF.Functions.Like(g.LastName ?? "", like) ||
                        EF.Functions.Like(g.Address ?? "", like) ||
                        EF.Functions.Like(g.City ?? "", like) ||
                        EF.Functions.Like(g.State ?? "", like) ||
                        EF.Functions.Like(g.Zipcode ?? "", like) ||
                        EF.Functions.Like(g.Email ?? "", like) ||
                        EF.Functions.Like(g.PhoneNumber ?? "", like))
                    .OrderBy(g => g.LastName).ThenBy(g => g.FirstName)
                    .Take(50)
                    .ToList();
            }

            return View(results);
        }

        // Select a Guest after searching
        [HttpGet]
        public IActionResult Select(long guestId, string returnUrl)
        {
            // Only allow local returnUrls
            if (!Url.IsLocalUrl(returnUrl))
                return BadRequest();

            // Append guestId to URL
            var sep = returnUrl.Contains('?') ? "&" : "?";
            return Redirect($"{returnUrl}{sep}guestId={guestId}");
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
                search = search.Trim();
                guests = guests.Where(g =>
                    EF.Functions.Like(g.FirstName, $"%{search}%") ||
                    EF.Functions.Like(g.LastName, $"%{search}%") ||
                    EF.Functions.Like(g.PhoneNumber, $"%{search}%") ||
                    EF.Functions.Like(g.Address, $"%{search}%") ||
                    EF.Functions.Like(g.City, $"%{search}%") ||
                    EF.Functions.Like(g.State, $"%{search}%") ||
                    EF.Functions.Like(g.Zipcode, $"%{search}%") ||
                    EF.Functions.Like(g.Email, $"%{search}%") ||
                    EF.Functions.Like(g.Notes, $"%{search}%") ||
                    EF.Functions.Like(g.Company, $"%{search}%")
                );

                // Go back to page 1 when searching
                page = 1;
            }

            // Sort
            guests = sort switch
            {
                "LastName" => dir == "asc"
                ? guests.OrderBy(g => g.LastName).ThenBy(g => g.FirstName).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.LastName).ThenBy(g => g.FirstName).ThenBy(g => g.GuestId),

                "PhoneNumber" => dir == "asc"
                ? guests.OrderBy(g => g.PhoneNumber).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.PhoneNumber).ThenBy(g => g.GuestId),

                "Address" => dir == "asc"
                ? guests.OrderBy(g => g.Address).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.Address).ThenBy(g => g.GuestId),

                "City" => dir == "asc"
                ? guests.OrderBy(g => g.City).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.City).ThenBy(g => g.GuestId),

                "State" => dir == "asc"
                ? guests.OrderBy(g => g.State).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.State).ThenBy(g => g.GuestId),

                "Zipcode" => dir == "asc"
                ? guests.OrderBy(g => g.Zipcode).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.Zipcode).ThenBy(g => g.GuestId),

                "Email" => dir == "asc"
                ? guests.OrderBy(g => g.Email).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.Email).ThenBy(g => g.GuestId),

                "Notes" => dir == "asc"
                ? guests.OrderBy(g => g.Notes).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.Notes).ThenBy(g => g.GuestId),

                "Company" => dir == "asc"
                ? guests.OrderBy(g => g.Company).ThenBy(g => g.GuestId)
                : guests.OrderByDescending(g => g.Company).ThenBy(g => g.GuestId),

                // Default sorting
                _ => guests.OrderBy(g => g.LastName).ThenBy(g => g.GuestId)
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
        public IActionResult Create(string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Guests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("GuestId,FirstName,LastName,PhoneNumber,Address,City,State,Zipcode,Email,Notes,Company")] Guest guest,
            string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(guest);
            }

            _context.Add(guest);
            await _context.SaveChangesAsync();

            // If not coming from Index, use returnUrl
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                // Append guestId to the returnUrl
                var separator = returnUrl.Contains('?') ? "&" : "?";
                return Redirect(returnUrl + $"{separator}guestId={guest.GuestId}");
            }

            return RedirectToAction(nameof(Index));
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
