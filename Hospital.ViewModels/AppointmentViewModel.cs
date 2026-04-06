using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
    }
}
