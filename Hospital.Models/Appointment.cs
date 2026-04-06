namespace Hospital.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; } // e.g., "10:00 AM"
        public AppointmentStatus Status { get; set; }
        public string? Description { get; set; }
        public string DoctorId { get; set; }
        public ApplicationUser Doctor { get; set; }
        public string PatientId { get; set; }
        public ApplicationUser Patient { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending, Confirmed, Cancelled
    }
}