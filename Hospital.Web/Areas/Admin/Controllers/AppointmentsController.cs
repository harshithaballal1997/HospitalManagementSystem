using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
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

        // ─── INDEX ──────────────────────────────────────────────────────────
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

        // ─── DETAILS ────────────────────────────────────────────────────────
        public IActionResult Details(int id)
        {
            var apt = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.Id == id, includeProperties: "Doctor,Patient")
                .FirstOrDefault();

            if (apt == null) return NotFound();

            return View(new AppointmentViewModel
            {
                Id = apt.Id,
                DoctorId = apt.DoctorId,
                PatientId = apt.PatientId,
                DoctorName = apt.Doctor?.Name ?? "N/A",
                PatientName = apt.Patient?.Name ?? "N/A",
                AppointmentDate = apt.AppointmentDate,
                TimeSlot = apt.TimeSlot,
                Status = apt.Status.ToString(),
                Description = apt.Description
            });
        }

        // ─── CREATE ─────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new AppointmentViewModel { AppointmentDate = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.DoctorId) || string.IsNullOrEmpty(vm.PatientId) || string.IsNullOrEmpty(vm.TimeSlot))
            {
                ModelState.AddModelError("", "Please fill in all required fields.");
                PopulateDropdowns();
                return View(vm);
            }

            var success = await _appointmentService.BookAppointmentAsync(vm);
            if (!success)
            {
                ModelState.AddModelError("", "This time slot is already taken. Please choose another.");
                PopulateDropdowns();
                return View(vm);
            }

            TempData["Success"] = "Appointment created successfully!";
            return RedirectToAction("Index");
        }

        // ─── EDIT / RESCHEDULE ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var apt = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.Id == id, includeProperties: "Doctor,Patient")
                .FirstOrDefault();

            if (apt == null) return NotFound();

            PopulateDropdowns(apt.DoctorId, apt.PatientId);
            return View(new AppointmentViewModel
            {
                Id = apt.Id,
                DoctorId = apt.DoctorId,
                PatientId = apt.PatientId,
                DoctorName = apt.Doctor?.Name,
                PatientName = apt.Patient?.Name,
                AppointmentDate = apt.AppointmentDate,
                TimeSlot = apt.TimeSlot,
                Status = apt.Status.ToString(),
                Description = apt.Description
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppointmentViewModel vm)
        {
            var apt = _unitOfWork.GenericRepository<Appointment>().GetById(vm.Id);
            if (apt == null) return NotFound();

            // Check for slot conflict (excluding self)
            bool conflict = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.DoctorId == vm.DoctorId
                    && x.AppointmentDate.Date == vm.AppointmentDate.Date
                    && x.TimeSlot == vm.TimeSlot
                    && x.Status != AppointmentStatus.Cancelled
                    && x.Id != vm.Id)
                .Any();

            if (conflict)
            {
                ModelState.AddModelError("", "That time slot is already taken. Please choose another.");
                PopulateDropdowns(vm.DoctorId, vm.PatientId);
                return View(vm);
            }

            apt.DoctorId = vm.DoctorId;
            apt.PatientId = vm.PatientId;
            apt.AppointmentDate = vm.AppointmentDate;
            apt.TimeSlot = vm.TimeSlot;
            apt.Description = vm.Description;
            apt.Status = Enum.TryParse<AppointmentStatus>(vm.Status, out var s) ? s : AppointmentStatus.Pending;

            _unitOfWork.GenericRepository<Appointment>().Update(apt);
            _unitOfWork.Save();

            TempData["Success"] = "Appointment rescheduled successfully!";
            return RedirectToAction("Index");
        }

        // ─── DELETE ──────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var apt = _unitOfWork.GenericRepository<Appointment>()
                .GetAll(filter: x => x.Id == id, includeProperties: "Doctor,Patient")
                .FirstOrDefault();

            if (apt == null) return NotFound();

            return View(new AppointmentViewModel
            {
                Id = apt.Id,
                DoctorName = apt.Doctor?.Name ?? "N/A",
                PatientName = apt.Patient?.Name ?? "N/A",
                AppointmentDate = apt.AppointmentDate,
                TimeSlot = apt.TimeSlot,
                Status = apt.Status.ToString(),
                Description = apt.Description
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var apt = _unitOfWork.GenericRepository<Appointment>().GetById(id);
            if (apt != null)
            {
                _unitOfWork.GenericRepository<Appointment>().Delete(apt);
                _unitOfWork.Save();
                TempData["Success"] = "Appointment deleted.";
            }
            return RedirectToAction("Index");
        }

        // ─── CANCEL (keep for quick inline cancel) ──────────────────────────
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

        // ─── AJAX: Get available slots ──────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(string doctorId, string date)
        {
            if (string.IsNullOrEmpty(date)) return Json(new string[] { });
            try
            {
                var dt = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, dt);
                return Json(slots);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // ─── HELPERS ────────────────────────────────────────────────────────
        private void PopulateDropdowns(string selectedDoctorId = null, string selectedPatientId = null)
        {
            var doctors = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(filter: x => x.IsDoctor)
                .Select(x => new SelectListItem { Value = x.Id, Text = $"Dr. {x.Name} ({x.Specialist})" });
            var patients = _unitOfWork.GenericRepository<ApplicationUser>()
                .GetAll(filter: x => !x.IsDoctor)
                .Select(x => new SelectListItem { Value = x.Id, Text = x.Name });

            ViewBag.Doctors = new SelectList(doctors, "Value", "Text", selectedDoctorId);
            ViewBag.Patients = new SelectList(patients, "Value", "Text", selectedPatientId);
            ViewBag.Statuses = new SelectList(Enum.GetNames(typeof(AppointmentStatus)));
        }
    }
}
