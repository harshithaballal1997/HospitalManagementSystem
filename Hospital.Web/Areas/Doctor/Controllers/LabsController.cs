using Hospital.Services;
using Hospital.ViewModels;
using Hospital.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hospital.Web.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class LabsController : Controller
    {
        private readonly ILabService _labService;
        private readonly IApplicationUserService _userService;
        private readonly IMedicalAssistantService _medicalAssistantService;
        private readonly ISafetyCheckerService _safetyCheckerService;

        public LabsController(ILabService labService, 
                              IApplicationUserService userService, 
                              IMedicalAssistantService medicalAssistantService,
                              ISafetyCheckerService safetyCheckerService)
        {
            _labService = labService;
            _userService = userService;
            _medicalAssistantService = medicalAssistantService;
            _safetyCheckerService = safetyCheckerService;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            return View(_labService.GetLatestLabsPaged(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.PatientId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userService.GetAllPatients(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(LabViewModel vm)
        {
            _labService.AddLab(vm);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vm = _labService.GetLabById(id);
            ViewBag.PatientId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_userService.GetAllPatients(), "Id", "Name", vm.PatientId);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(LabViewModel vm)
        {
            // Instead of overwriting the old record, we create a new longitudinal history entry.
            // Resetting the ID ensures EF Core performs an Insert instead of an Update.
            vm.Id = 0;
            vm.CreatedAt = DateTime.Now;
            _labService.AddLab(vm);
            
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _labService.DeleteLab(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetHistory(string patientId)
        {
            var history = _labService.GetLabsByPatientId(patientId);
            return PartialView("_PatientLabHistory", history.OrderByDescending(l => l.CreatedAt));
        }


        [HttpGet]
        public async Task<IActionResult> PatientSummary(string patientId)
        {
            var summary = await _medicalAssistantService.SummarizePatientHistoryAsync(patientId);
            return Content(summary);
        }

        [HttpGet]
        public async Task<IActionResult> CheckSafety(string patientId, string medicineName)
        {
            var result = await _safetyCheckerService.CheckPrescriptionSafetyAsync(patientId, medicineName);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeLab([FromBody] LabViewModel vm)
        {
            var result = await _safetyCheckerService.AnalyzeTestResultAsync(vm);
            return Json(result);
        }
    }
}
