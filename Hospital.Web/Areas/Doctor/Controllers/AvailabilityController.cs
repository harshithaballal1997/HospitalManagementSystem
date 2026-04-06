using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hospital.Web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class AvailabilityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public AvailabilityController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var doctorId = _userManager.GetUserId(User);
            var availabilities = _unitOfWork.GenericRepository<DoctorAvailability>()
                .GetAll(filter: x => x.DoctorId == doctorId)
                .OrderBy(x => x.DayOfWeek)
                .Select(x => new DoctorAvailabilityViewModel
                {
                    Id = x.Id,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    SlotDuration = x.SlotDuration
                }).ToList();

            // Ensure all 7 days are represented in a default state if empty
            if (!availabilities.Any())
            {
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    availabilities.Add(new DoctorAvailabilityViewModel { DayOfWeek = day, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(16, 0, 0) });
                }
            }

            return View(availabilities);
        }

        [HttpPost]
        public IActionResult Save(List<DoctorAvailabilityViewModel> model)
        {
            var doctorId = _userManager.GetUserId(User);
            var existing = _unitOfWork.GenericRepository<DoctorAvailability>()
                .GetAll(filter: x => x.DoctorId == doctorId).ToList();

            foreach (var item in model)
            {
                var dbItem = existing.FirstOrDefault(x => x.DayOfWeek == item.DayOfWeek);
                if (dbItem == null)
                {
                    _unitOfWork.GenericRepository<DoctorAvailability>().Add(new DoctorAvailability
                    {
                        DoctorId = doctorId,
                        DayOfWeek = item.DayOfWeek,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        SlotDuration = item.SlotDuration
                    });
                }
                else
                {
                    dbItem.StartTime = item.StartTime;
                    dbItem.EndTime = item.EndTime;
                    dbItem.SlotDuration = item.SlotDuration;
                    _unitOfWork.GenericRepository<DoctorAvailability>().Update(dbItem);
                }
            }

            _unitOfWork.Save();
            TempData["Success"] = "Weekly availability updated successfully!";
            return RedirectToAction("Index");
        }
    }
}
