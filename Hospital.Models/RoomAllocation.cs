using System;

namespace Hospital.Models
{
    public class RoomAllocation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int? BedId { get; set; }
        public Bed? Bed { get; set; }
        public string PatientId { get; set; }
        public ApplicationUser Patient { get; set; }
        public int HospitalId { get; set; }
        public HospitalInfo Hospital { get; set; }
        public AllocationStatus Status { get; set; }
        public DateTime AllocationDate { get; set; }
        public bool IsDischarged { get; set; }
    }

    public enum AllocationStatus
    {
        Reserved,
        Occupied
    }
}
