using Hospital.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.Services
{
    public interface IRoomAllocationService
    {
        void AllocateRoom(RoomAllocationViewModel vm);
        void UpdateStatus(int allocationId, AllocationStatus status);
        IEnumerable<RoomAllocationViewModel> GetAllAllocations();
        IEnumerable<RoomViewModel> GetAvailableRooms(int hospitalId, RoomType type);
        IEnumerable<BedViewModel> GetAvailableBeds(int roomId);
        void DischargePatient(int allocationId);
    }
}
