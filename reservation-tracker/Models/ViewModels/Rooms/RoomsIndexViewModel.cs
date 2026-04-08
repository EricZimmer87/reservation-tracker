using System.ComponentModel.DataAnnotations;

namespace reservation_tracker.Models.ViewModels.Rooms
{
    public class RoomsIndexViewModel
    {
        [Display(Name = "Room ID")]
        public long RoomId { get; set; }

        [Display(Name = "Room Number")]
        [Required]
        public string RoomNumber { get; set; } = "";

        [Display(Name = "Room Type")]
        [Required]
        public string RoomType { get; set; } = "";

        public string? Notes { get; set; }

        public bool CanDelete { get; set; }
    }
}
