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
    public class ApplicationUserService : IApplicationUserService
    {
        private IUnitOfWork _unitOfWork;

        public ApplicationUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PagedResult<ApplicationUserViewModel> GetAll(int PageNumber, int PageSize)
        {
            var vm = new ApplicationUserViewModel();
            int totalCount;
            List<ApplicationUserViewModel> vmList = new List<ApplicationUserViewModel>();
            try
            {
                int ExcludeRecords = (PageSize * PageNumber) - PageSize;
                var modelList = _unitOfWork.GenericRepository<ApplicationUser>().GetAll()
                    .Skip(ExcludeRecords)
                    .Take(PageSize)
                    .ToList();
                totalCount = _unitOfWork.GenericRepository<ApplicationUser>().GetAll().ToList().Count;
                vmList = ConverModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }
            var result = new PagedResult<ApplicationUserViewModel>
            {
                Data = vmList,
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalItems = totalCount
            };
            return result;
        }

        private List<ApplicationUserViewModel> ConverModelToViewModelList(List<ApplicationUser> modelList)
        {
            return modelList.Select(x => new ApplicationUserViewModel(x)).ToList();
        }

        public PagedResult<ApplicationUserViewModel> GetAllDoctor(int PageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PagedResult<ApplicationUserViewModel> GetAllPatient(int PageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PagedResult<ApplicationUserViewModel> SearchDoctor(int PageNumber, int pageSize, string Spicility)
        {
            throw new NotImplementedException();
        }
    }
}
