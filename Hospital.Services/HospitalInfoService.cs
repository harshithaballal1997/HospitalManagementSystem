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
    public class HospitalInfoService : IHospitalInfo
    {
        private IUnitOfWork _unitOfWork;

        public HospitalInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void DeleteHospitalInfo(int id)
        {
            var model = _unitOfWork.GenericRepository<HospitalInfo>().GetById(id);
            _unitOfWork.GenericRepository<HospitalInfo>().Delete(model);
            _unitOfWork.Save();
        }

        public PagedResult<HospitalInfoViewModel> GetAll(int pageNumber, int pageSize)
        {
            var vm = new HospitalInfoViewModel();
            int totalCount;
            List<HospitalInfoViewModel> vmList = new List<HospitalInfoViewModel>();
            try
            {
                int ExcludeRecords = (pageSize * pageNumber) - pageSize;

                var modelList = _unitOfWork.GenericRepository<HospitalInfo>().GetAll()
                    .Skip(ExcludeRecords).Take(pageSize).ToList();
                totalCount = _unitOfWork.GenericRepository<HospitalInfo>().GetAll().ToList().Count;
                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }
            var result = new PagedResult<HospitalInfoViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        public HospitalInfoViewModel GetHospitalById(int HospitalId)
        {
            var model = _unitOfWork.GenericRepository<HospitalInfo>().GetById(HospitalId);
            var vm = new HospitalInfoViewModel(model);
            return vm;
        }

        public void InsertHospitalInfo(HospitalInfoViewModel hositalInfo)
        {
            var model = new HospitalInfoViewModel().ConvertViewModel(hositalInfo);
            _unitOfWork.GenericRepository<HospitalInfo>().Add(model);
            _unitOfWork.Save();
        }

        public void UpdateHospitalInfo(HospitalInfoViewModel hospitalInfo)
        {
            var model = new HospitalInfoViewModel().ConvertViewModel(hospitalInfo);
            var ModelById = _unitOfWork.GenericRepository<HospitalInfo>().GetById(model.Id);
            ModelById.Name = hospitalInfo.Name;
            ModelById.City = hospitalInfo.City;
            ModelById.Pincode = hospitalInfo.Pincode;
            ModelById.Country = hospitalInfo.Country;
            _unitOfWork.GenericRepository<HospitalInfo>().Update(ModelById);
            _unitOfWork.Save();
        }

        private List<HospitalInfoViewModel> ConvertModelToViewModelList(List<HospitalInfo> modelList)
        {
            return modelList.Select(x => new HospitalInfoViewModel(x)).ToList();
        }

        //public IEnumerable<HospitalInfo> GetAllHospitals()
        //{
        //    //return _unitOfWork.GenericRepository<HospitalInfo>().GetAll();
        //}

        public IEnumerable<HospitalInfoViewModel> GetAllHospitals()
        {
            // 1. Get the list of Models from the database
            var modelList = _unitOfWork.GenericRepository<HospitalInfo>().GetAll();

            // 2. Map (convert) each Model into a ViewModel
            var viewModelList = modelList.Select(x => new HospitalInfoViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                City = x.City,
                Pincode = x.Pincode,
                Country = x.Country
            }).ToList();

            return viewModelList;
        }
    }
}
