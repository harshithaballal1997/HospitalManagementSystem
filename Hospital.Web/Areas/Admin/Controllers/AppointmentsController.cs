using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentsController(IAppointmentService appointmentService, IUnitOfWork unitOfWork)
        {
            _appointmentService = appointmentService;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var appointments = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(includeProperties: "Doctor,Patient")
                .OrderByDescending(x => x.AppointmentDate)
                .Select(x => new AppointmentViewModel
                {
                    Id = x.Id,
                    DoctorId = x.DoctorId,
                    PatientId = x.PatientId,
                    DoctorName = x.Doctor != null ? x.Doctor.Name : "N/A",
                    PatientName = x.Patient != null ? x.Patient.Name : "N/A",
                    AppointmentDate = x.AppointmentDate,
                    TimeSlot = x.TimeSlot,
                    Status = x.Status.ToString(),
                    Description = x.Description
                }).ToList();

            return View(appointments);
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var apt = _unitOfWork.GenericRepository<Appointment>().GetById(id);
            if (apt != null)
            {
                apt.Status = AppointmentStatus.Cancelled;
                _unitOfWork.GenericRepository<Appointment>().Update(apt);
                _unitOfWork.Save();
                TempData["Success"] = "Appointment cancelled.";
            }
            return RedirectToAction("Index");
        }
    }
}
