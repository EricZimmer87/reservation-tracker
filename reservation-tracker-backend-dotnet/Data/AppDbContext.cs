using Microsoft.EntityFrameworkCore;
using reservation_tracker_backend_dotnet.Models;

namespace reservation_tracker_backend_dotnet.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation>Reservations { get; set; }
    }
}
