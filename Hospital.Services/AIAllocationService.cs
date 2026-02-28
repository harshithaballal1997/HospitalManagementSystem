using Hospital.Models;
using Hospital.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public class AIAllocationService : IAIAllocationService
    {
        private readonly IRoomAllocationService _roomAllocationService;

        public AIAllocationService(IRoomAllocationService roomAllocationService)
        {
            _roomAllocationService = roomAllocationService;
        }

        public async Task<RoomAllocationViewModel> SuggestOptimalRoom(int hospitalId, RoomType preferredType)
        {
            // Get all available rooms of the preferred type
            var availableRooms = _roomAllocationService.GetAvailableRooms(hospitalId, preferredType);

            if (!availableRooms.Any())
            {
                return null;
            }

            // "AI" Logic: Find the room with the most available beds (lowest density)
            // This is "optimal" as it reduces crowding in shared rooms.
            var optimalRoom = availableRooms
                .OrderByDescending(r => _roomAllocationService.GetAvailableBeds(r.Id).Count())
                .First();

            var availableBeds = _roomAllocationService.GetAvailableBeds(optimalRoom.Id);
            var optimalBed = availableBeds.First();

            return new RoomAllocationViewModel
            {
                HospitalId = hospitalId,
                RoomId = optimalRoom.Id,
                RoomNumber = optimalRoom.RoomNumber,
                BedId = optimalBed.Id,
                BedNumber = optimalBed.BedNumber,
                Status = AllocationStatus.Reserved // Suggest as Reserved by default
            };
        }
    }
}
