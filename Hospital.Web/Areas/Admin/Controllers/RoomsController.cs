using Hospital.Services;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomsController : Controller
    {
        private IRoomService _roomService;
        private IHospitalInfo _hospitalInfo;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public IActionResult Index(int pageNumber=1, int pageSize=10)
        {
            return View(_roomService.GetAll(pageNumber,pageSize));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var viewModel = _roomService.GetRoomById(id);
            ViewBag.Hospitals = _roomService.GetAllHospitals().ToList();
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Edit(RoomViewModel vm)
        {
            _roomService.UpdateRoom(vm);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Create()
        {
            var hospitals = _roomService.GetAllHospitals().ToList();
            ViewBag.Hospitals = hospitals;
            return View();
        }
        [HttpPost]
        public IActionResult Create(RoomViewModel vm)
        {
            if(!ModelState.IsValid)
            {
                var hospitals = _roomService.GetAllHospitals().ToList();
                ViewBag.Hospitals = hospitals;
                
            }
            _roomService.InsertRoom(vm);
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            _roomService.DeleteRoom(id);
            return RedirectToAction("Index");
        }
    }
}
