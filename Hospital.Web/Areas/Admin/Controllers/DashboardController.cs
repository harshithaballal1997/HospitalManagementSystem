using Hospital.Models;
using Hospital.Services;
using Hospital.Utilities;
using Hospital.ViewModels;
using Hospital.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = WebSiteRoles.WebSite_Admin)]
    public class DashboardController : Controller
    {
        private readonly IHospitalInfo _hospitalService;
        private readonly IApplicationUserService _userService;
        private readonly IRoomAllocationService _allocationService;
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(
            IHospitalInfo hospitalService,
            IApplicationUserService userService,
            IRoomAllocationService allocationService,
            IUnitOfWork unitOfWork)
        {
            _hospitalService = hospitalService;
            _userService = userService;
            _allocationService = allocationService;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalHospitals = _hospitalService.GetAllHospitals().Count(),
                TotalPatients = _userService.GetAllPatients().Count(),
                TotalDoctors = _userService.GetAllDoctor(1, int.MaxValue).TotalItems
            };

            var hospitals = _unitOfWork.GenericRepository<Hospital.Models.HospitalInfo>().GetAll(includeProperties: "Rooms.Beds");
            foreach (var hospital in hospitals)
            {
                int totalBeds = 0;
                int occupiedBeds = 0;

                if (hospital.Rooms != null)
                {
                    foreach (var room in hospital.Rooms)
                    {
                        if (room.Beds != null)
                        {
                            totalBeds += room.Beds.Count;
                            occupiedBeds += room.Beds.Count(b => b.IsOccupied);
                        }
                    }
                }

                viewModel.HospitalAvailability.Add(new HospitalRoomAvailabilityViewModel
                {
                    HospitalName = hospital.Name,
                    TotalBeds = totalBeds,
                    AvailableBeds = totalBeds - occupiedBeds
                });
            }

            return View(viewModel);
        }
    }
}
