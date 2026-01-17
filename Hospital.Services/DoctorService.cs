using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Utilities;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public class DoctorService : IDoctorService
    {
        private IUnitOfWork _unitOfWork;
        public DoctorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void AddTiming(TimmingViewModel timming)
        {
            var model = new TimmingViewModel().ConvertViewModel(timming);
            _unitOfWork.GenericRepository<Timming>().Add(model);
            _unitOfWork.Save();
        }

        public void DeleteTiming(int TimmingId)
        {
            var model = _unitOfWork.GenericRepository<Timming>().GetById(TimmingId);
            _unitOfWork.GenericRepository<Timming>().Delete(model);
            _unitOfWork.Save();
        }

        public PagedResult<TimmingViewModel> GetAll(int pageNumber, int pageSize)
        {
            var vm = new TimmingViewModel();
            int totalCount;
            List<TimmingViewModel> vmList = new List<TimmingViewModel>();
            try
            {
                int ExcludeRecords = (pageSize * pageNumber) - pageSize;
                var modelList = _unitOfWork.GenericRepository<Timming>().GetAll().Skip(ExcludeRecords).Take(pageSize).ToList();
                totalCount = _unitOfWork.GenericRepository<Timming>().GetAll().ToList().Count;
                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }
            var result = new PagedResult<TimmingViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        private List<TimmingViewModel> ConvertModelToViewModelList(List<Timming> modelList)
        {
            return modelList.Select(x => new TimmingViewModel(x)).ToList();
        }

        public IEnumerable<TimmingViewModel> GetAll()
        {
            var TimmingList = _unitOfWork.GenericRepository<Timming>().GetAll().ToList();
            var vmList = ConvertModelToViewModelList(TimmingList);
            return vmList;
        }

        public TimmingViewModel GetTimmingById(int TimmingId)
        {
            var model = _unitOfWork.GenericRepository<Timming>().GetById(TimmingId);
            var vm = new TimmingViewModel(model);
            return vm;
        }

        public void UpdateTiming(TimmingViewModel timming)
        {
            var model = new TimmingViewModel().ConvertViewModel(timming);
            var ModelById = _unitOfWork.GenericRepository<Timming>().GetById(model.Id);
            ModelById.Id = timming.Id;
            ModelById.Doctor = timming.Doctor;
            ModelById.Status = Enum.Parse<Status>(timming.Status);
            ModelById.Duration = timming.Duration;
            ModelById.MorningShiftStartTime = timming.MorningShiftStartTime;
            ModelById.MorningShiftEndTime = timming.MorningShiftEndTime;
            ModelById.AfternoonShiftStartTime = timming.AfternoonShiftStartTime;
            ModelById.AfternoonShiftEndTime = timming.AfternoonShiftEndTime;
            _unitOfWork.GenericRepository<Timming>().Update(ModelById);
            _unitOfWork.Save();
        }
    }
}
