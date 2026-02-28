using Hospital.Models;
using System;

namespace Hospital.ViewModels
{
    public class RoomAllocationViewModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int? BedId { get; set; }
        public string? BedNumber { get; set; }
        public string PatientId { get; set; }
        public string? PatientName { get; set; }
        public int HospitalId { get; set; }
        public string? HospitalName { get; set; }
        public AllocationStatus Status { get; set; }
        public DateTime AllocationDate { get; set; }
        public bool IsDischarged { get; set; }

        public RoomAllocationViewModel() { }

        public RoomAllocationViewModel(RoomAllocation model)
        {
            Id = model.Id;
            RoomId = model.RoomId;
            RoomNumber = model.Room?.RoomNumber;
            BedId = model.BedId;
            BedNumber = model.Bed?.BedNumber;
            PatientId = model.PatientId;
            PatientName = model.Patient?.Name;
            HospitalId = model.HospitalId;
            HospitalName = model.Hospital?.Name;
            Status = model.Status;
            AllocationDate = model.AllocationDate;
            IsDischarged = model.IsDischarged;
        }

        public RoomAllocation ConvertViewModel(RoomAllocationViewModel model)
        {
            return new RoomAllocation
            {
                Id = model.Id,
                RoomId = model.RoomId,
                BedId = model.BedId,
                PatientId = model.PatientId,
                HospitalId = model.HospitalId,
                Status = model.Status,
                AllocationDate = model.AllocationDate,
                IsDischarged = model.IsDischarged
            };
        }
    }
}
