using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using reservation_tracker.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ReservationTrackerContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddAuthentication(options =>
    {
        // Use cookies
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Revalidate authenticated user against `Users` table on each request
        // Prevents banning a user, but the user still have access if still logged in.
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync();
                    return;
                }

                var db = context.HttpContext.RequestServices
                    .GetRequiredService<ReservationTrackerContext>();

                var user = await db.Users.FindAsync(userId);

                // User removed or banned
                if (user == null || user.IsBanned)
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync();
                    return;
                }

                // User is valid, do nothing
            }
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

        // Get the profile picture from Google, map it to "urn:google:picture"
        options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");

        // Google says who the person is. Should I allow them?
        options.Events.OnCreatingTicket = async context =>
        {
            // Get database context for ReservationTracker database
            var db = context.HttpContext.RequestServices
                .GetRequiredService<ReservationTrackerContext>();

            // Get the Google email
            var email =
                context.Principal?.FindFirstValue(ClaimTypes.Email) ??
                context.Principal?.FindFirstValue("email");

            // No email? Fail.
            if (string.IsNullOrWhiteSpace(email))
            {
                // If user is not allowed, kill that identity.
                context.Principal = null; // Important!
                context.Fail("Google did not provide an email address.");
                return;
            }

            email = email.Trim().ToLowerInvariant();

            // Look up user in the ReservationTracker database
            var user = await db.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email);

            // Deny access if that user is not found.
            if (user == null)
            {
                context.Principal = null;
                context.Fail("You are not authorized to use this application.");
                return;
            }

            // Deny access if that user is banned.
            if (user.IsBanned)
            {
                context.Principal = null;
                context.Fail("Your account has been disabled.");
                return;
            }

            // Get Google ID, display name, and picture from Google.
            var googleId =
                context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                context.Principal?.FindFirstValue("sub");

            var displayName =
                context.Principal?.FindFirstValue(ClaimTypes.Name) ??
                context.Principal?.FindFirstValue("name");

            var picture = context.Principal?.FindFirstValue("urn:google:picture");

            // Update the database if Google info changed
            var changed = false;

            if (!string.IsNullOrWhiteSpace(googleId) && user.GoogleId != googleId)
            {
                user.GoogleId = googleId;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(displayName) && user.DisplayName != displayName)
            {
                user.DisplayName = displayName;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(picture) && user.Picture != picture)
            {
                user.Picture = picture;
                changed = true;
            }

            if (changed)
            {
                await db.SaveChangesAsync();
            }

            // Stop using Google's identity and start using the app's identity.
            var claims = new List<Claim>
                {
                    // Use internal UserId
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    // The app's email
                    new Claim(ClaimTypes.Email, user.Email),
                    // What to display in the UI for user's name
                    new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email)
                };

            // Get the picture so it is stored in the cookie, so we can use without having to query database.
            if (!string.IsNullOrWhiteSpace(user.Picture))
            {
                claims.Add(new Claim("picture", user.Picture));
            }

            // Determine if user is admin
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            // Create a new identity using the app's claims and attach it to the cookie auth scheme.
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Replace the Google principal.
            context.Principal = new ClaimsPrincipal(identity);
        };

        // If Google auth fails, send user to /Account/AccessDenied
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/Account/AccessDenied");
            context.HandleResponse();
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().Add(endpointBuilder =>
    endpointBuilder.Metadata.Add(new AllowAnonymousAttribute()));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();