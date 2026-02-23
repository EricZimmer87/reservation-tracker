using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Rooms
{
    public class RoomsIndexViewModel
    {
        [Display(Name = "Room ID")]
        public long RoomId { get; set; }
        [Display(Name = "Room Number")]
        public string RoomNumber { get; set; }
        [Display(Name = "Room Type")]
        public string RoomType { get; set; }

        public string? Notes { get; set; }
    }
}
