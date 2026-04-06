using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<string>> GetAvailableSlotsAsync(string doctorId, DateTime date);
        Task<bool> BookAppointmentAsync(AppointmentViewModel vm);
        Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByDoctorAsync(string doctorId);
        Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByPatientAsync(string patientId);
    }
}
