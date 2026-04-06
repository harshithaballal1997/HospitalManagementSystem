using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Web.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize(Roles = "Patient")]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingController(IUnitOfWork unitOfWork, 
                                 IAppointmentService appointmentService, 
                                 UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.HospitalId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_unitOfWork.GenericRepository<HospitalInfo>().GetAll(), "Id", "Name");
            return View();
        }

        [HttpGet]
        public IActionResult GetDoctorsByHospital(int hospitalId)
        {
            var doctors = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(filter: x => x.IsDoctor && x.Address.Contains(_unitOfWork.GenericRepository<HospitalInfo>().GetById(hospitalId).Name))
                .Select(x => new { id = x.Id, name = x.Name + " (" + x.Specialist + ")" })
                .ToList();
            
            return Json(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(string doctorId, string date)
        {
            if (string.IsNullOrEmpty(date)) return Json(new List<string>());
            
            DateTime appointmentDate = DateTime.Parse(date);
            var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, appointmentDate);
            return Json(slots);
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] AppointmentViewModel vm)
        {
            vm.PatientId = _userManager.GetUserId(User);
            var success = await _appointmentService.BookAppointmentAsync(vm);
            
            if (success)
                return Json(new { success = true, message = "Appointment booked successfully!" });
            
            return Json(new { success = false, message = "This slot is no longer available. Please pick another one." });
        }
    }
}
