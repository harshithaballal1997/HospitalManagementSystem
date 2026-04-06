using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<string>> GetAvailableSlotsAsync(string doctorId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;
            var availability = _unitOfWork.GenericRepository<DoctorAvailability>()
                .GetAll(filter: x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek)
                .FirstOrDefault();

            if (availability == null)
                return Enumerable.Empty<string>();

            // Generate all possible slots
            var slots = new List<string>();
            var currentTime = availability.StartTime;
            while (currentTime.Add(TimeSpan.FromMinutes(availability.SlotDuration)) <= availability.EndTime)
            {
                slots.Add(DateTime.Today.Add(currentTime).ToString("hh:mm tt"));
                currentTime = currentTime.Add(TimeSpan.FromMinutes(availability.SlotDuration));
            }

            // Get existing appointments for that day
            var existingAppointments = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.DoctorId == doctorId && x.AppointmentDate.Date == date.Date && x.Status != AppointmentStatus.Cancelled)
                .Select(x => x.TimeSlot)
                .ToList();

            // Return available slots
            return slots.Except(existingAppointments);
        }

        public async Task<bool> BookAppointmentAsync(AppointmentViewModel vm)
        {
            // Re-verify slot is still available (prevent double-booking)
            var availableSlots = await GetAvailableSlotsAsync(vm.DoctorId, vm.AppointmentDate);
            if (!availableSlots.Contains(vm.TimeSlot))
                return false;

            var appointment = new Appointment
            {
                DoctorId = vm.DoctorId,
                PatientId = vm.PatientId,
                AppointmentDate = vm.AppointmentDate,
                TimeSlot = vm.TimeSlot,
                Description = vm.Description,
                Status = AppointmentStatus.Pending
            };

            await _unitOfWork.GenericRepository<Appointment>().AddAsync(appointment);
            _unitOfWork.Save();
            return true;
        }

        public async Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByDoctorAsync(string doctorId)
        {
            var list = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.DoctorId == doctorId, includeProperties: "Patient,Doctor");
            
            return list.Select(x => new AppointmentViewModel
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                DoctorName = x.Doctor.Name,
                PatientName = x.Patient.Name,
                AppointmentDate = x.AppointmentDate,
                TimeSlot = x.TimeSlot,
                Status = x.Status.ToString(),
                Description = x.Description
            });
        }

        public async Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByPatientAsync(string patientId)
        {
            var list = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.PatientId == patientId, includeProperties: "Doctor,Patient");

            return list.Select(x => new AppointmentViewModel
            {
                Id = x.Id,
                DoctorId = x.DoctorId,
                PatientId = x.PatientId,
                DoctorName = x.Doctor.Name,
                PatientName = x.Patient.Name,
                AppointmentDate = x.AppointmentDate,
                TimeSlot = x.TimeSlot,
                Status = x.Status.ToString(),
                Description = x.Description
            });
        }
    }
}
