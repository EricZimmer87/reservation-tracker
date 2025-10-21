namespace reservation_tracker_backend_dotnet.Data;

using reservation_tracker_backend_dotnet.Models;
using Microsoft.EntityFrameworkCore;

// Helper to seed database
public static class DbSeeder
{
    private static readonly List<Room> SeedRooms =
    [
        new Room { RoomNumber = "101", RoomType = "Double Queen" },
        new Room { RoomNumber = "102", RoomType = "Double Queen" },
        new Room { RoomNumber = "103", RoomType = "Double Queen" },
        new Room { RoomNumber = "104", RoomType = "Double Queen" },
        new Room { RoomNumber = "105", RoomType = "Double Queen" },
        new Room { RoomNumber = "106", RoomType = "Double Queen" },
        new Room { RoomNumber = "107", RoomType = "Double Queen" },
        new Room { RoomNumber = "108", RoomType = "Double Queen" },
        new Room { RoomNumber = "109", RoomType = "Double Queen" },
        new Room { RoomNumber = "110", RoomType = "Handicap Single Queen" },
        new Room { RoomNumber = "111", RoomType = "Double Queen" },
        new Room { RoomNumber = "112", RoomType = "Double Queen" },
        new Room { RoomNumber = "113", RoomType = "Double Queen" },
        new Room { RoomNumber = "114", RoomType = "Double Queen" },
        new Room { RoomNumber = "115", RoomType = "King Suite" },
        new Room { RoomNumber = "116", RoomType = "King Suite" }
    ];

    private static readonly List<Guest> SeedGuests =
    [
        new Guest
        {
            FirstName = "John",
            LastName = "Smith",
            PhoneNumber = "402-555-1234",
            Address = "123 Main St",
            City = "Lincoln",
            State = "NE",
            Zipcode = "68508",
            Email = "john.smith@example.com",
            Notes = "Prefers east rooms."
        },
        new Guest
        {
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "308-555-7890",
            Address = "456 Elm St",
            City = "Kearney",
            State = "NE",
            Zipcode = "68845",
            Email = "jane.doe@example.com",
            Notes = "Late check-in around 11 PM"
        },
        new Guest
        {
            FirstName = "Michael",
            LastName = "Brown",
            PhoneNumber = "531-555-4444",
            Address = "789 Maple Ave",
            City = "Grand Island",
            State = "NE",
            Zipcode = "68801",
            Notes = "Allergic to pets",
            Company = "Brown Logistics"
        },
        new Guest
        {
            FirstName = "Emily",
            LastName = "Johnson",
            PhoneNumber = "402-555-8888",
            Address = "135 Oak St",
            City = "Columbus",
            State = "NE",
            Zipcode = "68601",
            Email = "emilyj@techmail.com"
        },
        new Guest
        {
            FirstName = "Sara",
            LastName = "Nguyen",
            PhoneNumber = "308-555-9876",
            Address = "980 River Rd",
            City = "Scottsbluff",
            State = "NE",
            Zipcode = "69361",
            Notes = "Always pays cash",
            Company = "Sara Realty"
        },
        new Guest
        {
            FirstName = "David",
            LastName = "Lee",
            PhoneNumber = "531-555-2233",
            Address = "22 Pine St",
            City = "Norfolk",
            State = "NE",
            Zipcode = "68701",
            Email = "dlee@example.com",
            Company = "Lee Construction"
        },
        new Guest
        {
            FirstName = "Karen",
            LastName = "Thompson",
            PhoneNumber = "402-555-0000",
            Address = "742 Willow Dr",
            City = "Hastings",
            State = "NE",
            Zipcode = "68901",
            Notes = "Talks a lot"
        }
    ];
    
    public static void Seed(DbContext context)
    {
        // User - Admin
       if (!context.Set<User>().Any())
       {
           context.Set<User>().Add(
               new User { Email = "ericzimmer87@gmail.com", IsAdmin = true }
           );
           context.SaveChanges();
       }
       // Rooms
       if(!context.Set<Room>().Any())
       {
           context.Set<Room>().AddRange(SeedRooms);
           context.SaveChanges();
       }
       
        // Guests
        if (!context.Set<Guest>().Any())
        {
            context.Set<Guest>().AddRange(SeedGuests);
            context.SaveChanges();
        }
    }

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken = default)
    {
        // User - Admin
       var userExists = await context.Set<User>().AnyAsync(cancellationToken);
       if (!userExists)
       {
           context.Set<User>().Add(
               new User { Email = "ericzimmer87@gmail.com", IsAdmin = true }
           );
           await context.SaveChangesAsync(cancellationToken);
       }
       
       // Rooms
       var roomsExist = await context.Set<Room>().AnyAsync(cancellationToken);
       if (!roomsExist)
       {
           context.Set<Room>().AddRange(SeedRooms);
           await context.SaveChangesAsync(cancellationToken);
       }
       
       // Guests
       var guestsExist = await context.Set<Guest>().AnyAsync(cancellationToken);
        if (!guestsExist)
        {
            context.Set<Guest>().AddRange(SeedGuests);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}