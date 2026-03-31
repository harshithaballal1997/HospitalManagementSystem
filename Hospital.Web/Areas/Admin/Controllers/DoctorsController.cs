using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Hospital.Utilities;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebSiteRoles.WebSite_Admin)]
    public class DoctorsController : Controller
    {
        private readonly IApplicationUserService _userService;

        public DoctorsController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index(int pageNumber = 1, int pageSize = 40)
        {
            var result = _userService.GetAllDoctor(pageNumber, pageSize);
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ApplicationUserViewModel { IsDoctor = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUserViewModel viewModel)
        {
            viewModel.IsDoctor = true; // Ensure it's a doctor
            await _userService.CreateUserAsync(viewModel);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var viewModel = _userService.GetUserById(id);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(ApplicationUserViewModel viewModel)
        {
            _userService.UpdateUser(viewModel);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction("Index");
        }
    }
}
