using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactsController : Controller
    {
        private IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            return View(_contactService.GetAll(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var viewModel = _contactService.GetContactById(id);
            ViewBag.Hospitals = _contactService.GetAllHospitals().ToList();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(ContactViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var hospitals = _contactService.GetAllHospitals().ToList();
                ViewBag.Hospitals = hospitals;

            }
            _contactService.UpdateContact(vm);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Create()
        {
            var hospitals = _contactService.GetAllHospitals().ToList();
            ViewBag.Hospitals = hospitals;
            return View();
        }
        [HttpPost]
        public IActionResult Create(ContactViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var hospitals = _contactService.GetAllHospitals().ToList();
                ViewBag.Hospitals = hospitals;

            }
            _contactService.InsertContact(vm);
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            _contactService.DeleteContact(id);
            return RedirectToAction("Index");
        }
    }
}
