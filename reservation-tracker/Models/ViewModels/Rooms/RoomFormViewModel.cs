namespace reservation_tracker.Models.ViewModels.Rooms
{
    public class RoomFormViewModel
    {
        public long RoomId { get; set; }

        public string RoomNumber { get; set; }

        public string RoomType { get; set; }

        public string? Notes { get; set; }

        public List<String> RoomTypeList { get; set; } = new();
    }
}
