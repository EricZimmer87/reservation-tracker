using Microsoft.EntityFrameworkCore;

namespace reservation_tracker_backend_dotnet.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
