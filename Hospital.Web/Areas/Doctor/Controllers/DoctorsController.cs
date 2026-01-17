using Hospital.Models;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

namespace Hospital.Web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            return View(_doctorService.GetAll(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult AddTimming()
        {
            Timming timming = new Timming();
            List<SelectListItem> morningShiftStart = new List<SelectListItem>();
            List<SelectListItem> morningShiftEnd = new List<SelectListItem>();
            List<SelectListItem> afternoonShiftStart = new List<SelectListItem>();
            List<SelectListItem> afternoonShiftEnd = new List<SelectListItem>();

            for (int i=1; i<= 11; i++)
            {
                morningShiftStart.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            for (int i = 1; i <= 13; i++)
            {
                morningShiftEnd.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            for (int i = 13; i <= 16; i++)
            {
                afternoonShiftStart.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            for (int i = 13; i <= 18; i++)
            {
                afternoonShiftEnd.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.morningStart = new SelectList(morningShiftStart, "Value", "Text");
            ViewBag.morningEnd = new SelectList(morningShiftEnd, "Value", "Text");
            ViewBag.afternoonStart = new SelectList(afternoonShiftStart, "Value", "Text");
            ViewBag.afternoonEnd = new SelectList(afternoonShiftEnd, "Value", "Text");

            TimmingViewModel vm = new TimmingViewModel();
            vm.ScheduleDate = DateTime.Now;
            vm.ScheduleDate = vm.ScheduleDate.AddDays(1);
            return View();
        }

        [HttpPost]
        public IActionResult AddTimming(TimmingViewModel vm)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var Claims = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (Claims != null)
            {
                vm.Doctor.Id = Claims.Value;
                _doctorService.AddTiming(vm);
                
            }            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var viewModel = _doctorService.GetTimmingById(id);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(TimmingViewModel vm)
        {
            _doctorService.UpdateTiming(vm);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _doctorService.DeleteTiming(id);
            return RedirectToAction("Index");
        }
    }
}
