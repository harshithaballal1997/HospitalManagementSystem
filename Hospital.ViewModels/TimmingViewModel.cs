using Hospital.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class TimmingViewModel
    {
        public int Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int MorningShiftStartTime { get; set; }
        public int MorningShiftEndTime { get; set; }
        public int AfternoonShiftStartTime { get; set; }
        public int AfternoonShiftEndTime { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }

        List<SelectListItem> morningShiftStart = new List<SelectListItem>();
        List<SelectListItem> morningShiftEnd = new List<SelectListItem>();
        List<SelectListItem> AfternoonShiftStart = new List<SelectListItem>();
        List<SelectListItem> AfternoonShiftEnd = new List<SelectListItem>();

        public ApplicationUser Doctor { get; set; }

        public TimmingViewModel()
        {

        }

        public TimmingViewModel(Timming model)
        {
            Id = model.Id;
            ScheduleDate = model.Date;
            MorningShiftStartTime = model.MorningShiftStartTime;
            MorningShiftEndTime = model.MorningShiftEndTime;
            AfternoonShiftStartTime = model.AfternoonShiftStartTime;
            AfternoonShiftEndTime = model.AfternoonShiftEndTime;
            Duration = model.Duration;
            Status = model.Status.ToString();
            Doctor = model.Doctor;
        }
        public Timming ConvertViewModel(TimmingViewModel model)
        {
            return new Timming
            {
                Id = model.Id,
                Date = model.ScheduleDate,
                MorningShiftStartTime = model.MorningShiftStartTime,
                MorningShiftEndTime = model.MorningShiftEndTime,
                AfternoonShiftStartTime = model.AfternoonShiftStartTime,
                AfternoonShiftEndTime = model.AfternoonShiftEndTime,
                Duration = model.Duration,
                Status = Enum.Parse<Status>(model.Status),
                Doctor = model.Doctor
            };
        }
    }
}
