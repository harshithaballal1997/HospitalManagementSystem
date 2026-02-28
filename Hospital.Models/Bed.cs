namespace Hospital.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public string BedNumber { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public bool IsOccupied { get; set; }
    }
}
