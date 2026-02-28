using System.Collections.Generic;

namespace Hospital.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalHospitals { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public List<HospitalRoomAvailabilityViewModel> HospitalAvailability { get; set; } = new List<HospitalRoomAvailabilityViewModel>();
    }

    public class HospitalRoomAvailabilityViewModel
    {
        public string HospitalName { get; set; }
        public int TotalBeds { get; set; }
        public int AvailableBeds { get; set; }
        public int OccupiedBeds => TotalBeds - AvailableBeds;
    }
}
