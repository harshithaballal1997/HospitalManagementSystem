using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Utilities;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services
{
    public class RoomAllocationService : IRoomAllocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomAllocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AllocateRoom(RoomAllocationViewModel vm)
        {
            var model = new RoomAllocationViewModel().ConvertViewModel(vm);
            model.AllocationDate = DateTime.Now;
            model.IsDischarged = false;

            _unitOfWork.GenericRepository<RoomAllocation>().Add(model);

            if (model.Status == AllocationStatus.Occupied && model.BedId.HasValue)
            {
                var bed = _unitOfWork.GenericRepository<Bed>().GetById(model.BedId.Value);
                if (bed != null)
                {
                    bed.IsOccupied = true;
                    _unitOfWork.GenericRepository<Bed>().Update(bed);
                }
            }

            _unitOfWork.Save();
        }

        public void UpdateStatus(int allocationId, AllocationStatus status)
        {
            var allocation = _unitOfWork.GenericRepository<RoomAllocation>().GetById(allocationId);
            if (allocation != null)
            {
                allocation.Status = status;
                if (status == AllocationStatus.Occupied && allocation.BedId.HasValue)
                {
                    var bed = _unitOfWork.GenericRepository<Bed>().GetById(allocation.BedId.Value);
                    if (bed != null)
                    {
                        bed.IsOccupied = true;
                        _unitOfWork.GenericRepository<Bed>().Update(bed);
                    }
                }
                _unitOfWork.GenericRepository<RoomAllocation>().Update(allocation);
                _unitOfWork.Save();
            }
        }

        public IEnumerable<RoomAllocationViewModel> GetAllAllocations()
        {
            var allocations = _unitOfWork.GenericRepository<RoomAllocation>().GetAll(includeProperties: "Room,Bed,Patient,Hospital");
            return allocations.Select(x => new RoomAllocationViewModel(x)).ToList();
        }

        public PagedResult<RoomAllocationViewModel> GetAll(int pageNumber, int pageSize)
        {
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;
            var modelList = _unitOfWork.GenericRepository<RoomAllocation>().GetAll(includeProperties: "Room,Bed,Patient,Hospital")
                .Skip(ExcludeRecords).Take(pageSize).ToList();
            var totalCount = _unitOfWork.GenericRepository<RoomAllocation>().GetAll().Count();

            var vmList = modelList.Select(x => new RoomAllocationViewModel(x)).ToList();

            var result = new PagedResult<RoomAllocationViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        public IEnumerable<RoomViewModel> GetAvailableRooms(int hospitalId, RoomType type)
        {
            var rooms = _unitOfWork.GenericRepository<Room>().GetAll(x => x.HospitalId == hospitalId && x.RoomType == type, includeProperties: "Beds");
            
            // Filter rooms that have at least one available bed
            var availableRooms = new List<Room>();
            foreach (var room in rooms)
            {
                var beds = GetAvailableBeds(room.Id);
                if (beds.Any())
                {
                    availableRooms.Add(room);
                }
            }

            return availableRooms.Select(x => new RoomViewModel(x)).ToList();
        }

        public IEnumerable<BedViewModel> GetAvailableBeds(int roomId)
        {
            // Get all beds for the room
            var beds = _unitOfWork.GenericRepository<Bed>().GetAll(x => x.RoomId == roomId).ToList();
            
            // Get all active allocations for the room
            var activeAllocations = _unitOfWork.GenericRepository<RoomAllocation>().GetAll(x => x.RoomId == roomId && !x.IsDischarged).ToList();
            
            // A bed is available if it's not in the active allocations list
            var assignedBedIds = activeAllocations.Where(a => a.BedId.HasValue).Select(a => a.BedId.Value).ToList();
            
            var availableBeds = beds.Where(b => !assignedBedIds.Contains(b.Id)).ToList();
            
            return availableBeds.Select(x => new BedViewModel(x)).ToList();
        }

        public void DischargePatient(int allocationId)
        {
            var allocation = _unitOfWork.GenericRepository<RoomAllocation>().GetById(allocationId);
            if (allocation != null)
            {
                allocation.IsDischarged = true;
                if (allocation.BedId.HasValue)
                {
                    var bed = _unitOfWork.GenericRepository<Bed>().GetById(allocation.BedId.Value);
                    if (bed != null)
                    {
                        bed.IsOccupied = false;
                        _unitOfWork.GenericRepository<Bed>().Update(bed);
                    }
                }
                _unitOfWork.GenericRepository<RoomAllocation>().Update(allocation);
                _unitOfWork.Save();
            }
        }
    }
}
