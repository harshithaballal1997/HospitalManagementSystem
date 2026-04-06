using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class DoctorAvailability
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public ApplicationUser Doctor { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDuration { get; set; } = 30; // Minutes
    }
}
