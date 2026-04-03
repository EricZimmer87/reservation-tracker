using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using reservation_tracker.Data;
using reservation_tracker.Models;
using reservation_tracker.Models.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace reservation_tracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ReservationTrackerContext _context;

        // Get currently logged in UserId
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User is not authenticated.");

            if (!long.TryParse(userIdClaim, out var currentUserId))
                throw new Exception("Invalid user ID in claims.");

            return currentUserId;
        }

        public UsersController(ReservationTrackerContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserDetailsViewModel
            {
                UserId = user.UserId,
                GoogleId = user.GoogleId,
                DisplayName = user.DisplayName,
                Picture = user.Picture,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                IsBanned = user.IsBanned
            };

            return View(model);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var model = new UserCreateViewModel
            {
                IsAdmin = false,
                IsBanned = false
            };

            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim().ToLower();

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "A user with that email already exists.");
                return View(model);
            }

            var entity = new User
            {
                Email = model.Email,
                IsAdmin = model.IsAdmin,
                IsBanned = model.IsBanned
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserEditViewModel
            {
                UserId = user.UserId,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                IsBanned = user.IsBanned
            };

            return View(model);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, UserEditViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var normalizedEmail = model.Email.Trim().ToLower();

            var emailExists = await _context.Users
                .AnyAsync(u => u.UserId != id && u.Email.ToLower() == normalizedEmail);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "A user with that email already exists.");
                return View(model);
            }

            var currentUserId = GetCurrentUserId();

            // Admins cannot ban themselves
            if (id == currentUserId && model.IsBanned)
            {
                ModelState.AddModelError("IsBanned", "You cannot ban yourself.");
                return View(model);
            }

            // Admins cannot remove their own admin flag
            if (id == currentUserId && !model.IsAdmin)
            {
                ModelState.AddModelError("IsAdmin", "You cannot remove your own admin access.");
                return View(model);
            }

            user.Email = normalizedEmail;
            user.IsAdmin = model.IsAdmin;
            user.IsBanned = model.IsBanned;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
