using Hospital.Models;
using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Hospital.Utilities;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebSiteRoles.WebSite_Admin)]
    public class RoomAllocationController : Controller
    {
        private readonly IRoomAllocationService _roomAllocationService;
        private readonly IHospitalInfo _hospitalService;
        private readonly IApplicationUserService _userService;
        private readonly IAIAllocationService _aiService;

        public RoomAllocationController(
            IRoomAllocationService roomAllocationService,
            IHospitalInfo hospitalService,
            IApplicationUserService userService,
            IAIAllocationService aiService)
        {
            _roomAllocationService = roomAllocationService;
            _hospitalService = hospitalService;
            _userService = userService;
            _aiService = aiService;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            var allocations = _roomAllocationService.GetAll(pageNumber, pageSize);
            return View(allocations);
        }

        [HttpGet]
        public IActionResult Allocate()
        {
            ViewBag.Hospitals = new SelectList(_hospitalService.GetAllHospitals(), "Id", "Name");
            ViewBag.Patients = new SelectList(_userService.GetAllPatients(), "Id", "Name");
            return View(new RoomAllocationViewModel());
        }

        [HttpPost]
        public IActionResult Allocate(RoomAllocationViewModel vm)
        {
            if (vm.RoomId <= 0 || string.IsNullOrEmpty(vm.PatientId))
            {
                ModelState.AddModelError("", "Please select a Hospital, Patient, and a valid Room.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _roomAllocationService.AllocateRoom(vm);
                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred during allocation: " + ex.Message);
                }
            }

            ViewBag.Hospitals = new SelectList(_hospitalService.GetAllHospitals(), "Id", "Name");
            ViewBag.Patients = new SelectList(_userService.GetAllPatients(), "Id", "Name");
            return View(vm);
        }

        [HttpGet]
        public IActionResult GetRooms(int hospitalId, RoomType type)
        {
            var rooms = _roomAllocationService.GetAvailableRooms(hospitalId, type);
            return Json(rooms.Select(r => new { r.Id, r.RoomNumber }));
        }

        [HttpGet]
        public IActionResult GetBeds(int roomId)
        {
            var beds = _roomAllocationService.GetAvailableBeds(roomId);
            return Json(beds.Select(b => new { b.Id, b.BedNumber }));
        }

        [HttpGet]
        public async Task<IActionResult> SuggestOptimal(int hospitalId, RoomType type)
        {
            var suggestion = await _aiService.SuggestOptimalRoom(hospitalId, type);
            if (suggestion == null)
            {
                return Json(new { success = false, message = "No available rooms for this type." });
            }
            return Json(new { success = true, data = suggestion });
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, AllocationStatus status)
        {
            _roomAllocationService.UpdateStatus(id, status);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Discharge(int id)
        {
            _roomAllocationService.DischargePatient(id);
            return RedirectToAction("Index");
        }
    }
}
