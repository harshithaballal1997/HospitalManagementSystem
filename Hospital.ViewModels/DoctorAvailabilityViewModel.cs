using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hospital.ViewModels
{
    public class DoctorAvailabilityViewModel
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }
        
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
        
        [Display(Name = "Slot Duration (Mins)")]
        public int SlotDuration { get; set; } = 30;
    }

    public class WeeklyAvailabilityViewModel
    {
        public List<DoctorAvailabilityViewModel> Days { get; set; } = new List<DoctorAvailabilityViewModel>();
    }
}
