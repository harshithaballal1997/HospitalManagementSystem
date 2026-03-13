using System.Collections.Generic;

namespace Hospital.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Type { get; set; } // Keeping existing Type for compatibility if needed, but will use RoomType enum for logic
        public RoomType RoomType { get; set; }
        public string Status { get; set; }
        public int HospitalId { get; set; }
        public HospitalInfo Hospital { get; set; }
        public ICollection<Bed> Beds { get; set; } = new List<Bed>();
    }

    public enum RoomType
    {
        General,
        Double,
        Private,
        ICU,
        Ward
    }
}