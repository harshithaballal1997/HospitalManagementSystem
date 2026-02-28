using Hospital.Models;
using Hospital.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IBedStatusHandler
    {
        Task UpdateBedStatusFromSensorAsync(int roomId, double pressureValue);
    }

    public class BedStatusHandler : IBedStatusHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private const double OCCUPIED_THRESHOLD = 5.0; // kg or pressure unit

        public BedStatusHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateBedStatusFromSensorAsync(int roomId, double pressureValue)
        {
            var room = _unitOfWork.GenericRepository<Room>().GetById(roomId);
            if (room != null)
            {
                string newStatus = pressureValue > OCCUPIED_THRESHOLD ? "Occupied" : "Free";
                
                if (room.Status != newStatus)
                {
                    room.Status = newStatus;
                    _unitOfWork.GenericRepository<Room>().Update(room);
                    _unitOfWork.Save();
                    
                    // Here we could trigger a notification to the ADT system
                    // or automatically update the "Waiting Rectangle" status.
                }
            }
        }
    }
}
