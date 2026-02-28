using Hospital.Services;
using Microsoft.AspNetCore.Mvc;
using Hospital.Utilities;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebSiteRoles.WebSite_Admin)]
    public class UsersController : Controller
    {
        private IApplicationUserService _userService;
        public UsersController(IApplicationUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index(int PageNumber=1, int PageSize=10)
        {
            return View(_userService.GetAll(PageNumber,PageSize));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUserViewModel viewModel)
        {
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
        [HttpGet]
        public async Task<IActionResult> Seed()
        {
            string[] names = { "Alice Johnson", "Bob Smith", "Charlie Brown", "Diana Prince", "Edward Norton", "Fiona Gallagher", "George Clooney", "Hannah Abbott", "Ian Wright", "Julia Roberts", "Kevin Hart", "Laura Palmer", "Michael Scott", "Natalie Portman", "Oscar Issac", "Peter Parker", "Quinn Fabray", "Rachel Green", "Steve Rogers", "Tina Fey", "Uma Thurman", "Victor Stone", "Wanda Maximoff", "Xavier Woods", "Yara Shahidi", "Zane Grey", "Arthur Dent", "Bela Lugosi", "Clara Oswald", "Daisy Johnson" };
            string[] cities = { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
            Random rand = new Random();

            for (int i = 0; i < names.Length; i++)
            {
                var viewModel = new ApplicationUserViewModel
                {
                    Name = names[i],
                    Email = $"patient{i + 1}_{Guid.NewGuid().ToString().Substring(0, 4)}@example.com",
                    City = cities[rand.Next(cities.Length)],
                    Gender = (Hospital.Models.Gender)(rand.Next(3)),
                    IsDoctor = false,
                    Password = "Patient@123!"
                };
                await _userService.CreateUserAsync(viewModel);
            }
            return Content("Seed successful! 30 patients created.");
        }
    }
}
