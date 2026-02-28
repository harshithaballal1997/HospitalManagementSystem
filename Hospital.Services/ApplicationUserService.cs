using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Utilities;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Hospital.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationUserService(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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
                    .Where(x => x.IsDoctor == false)
                    .Skip(ExcludeRecords)
                    .Take(PageSize)
                    .ToList();
                totalCount = _unitOfWork.GenericRepository<ApplicationUser>().GetAll().Count(x => x.IsDoctor == false);
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

        public PagedResult<ApplicationUserViewModel> GetAllDoctor(int PageNumber, int PageSize)
        {
            var vm = new ApplicationUserViewModel();
            int totalCount;
            List<ApplicationUserViewModel> vmList = new List<ApplicationUserViewModel>();
            try
            {
                int ExcludeRecords = (PageSize * PageNumber) - PageSize;
                var modelList = _unitOfWork.GenericRepository<ApplicationUser>().GetAll()
                    .Where(x => x.IsDoctor == true)
                    .Skip(ExcludeRecords)
                    .Take(PageSize)
                    .ToList();
                totalCount = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(x=>x.IsDoctor==true).ToList().Count;
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

        public PagedResult<ApplicationUserViewModel> GetAllPatient(int PageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PagedResult<ApplicationUserViewModel> SearchDoctor(int PageNumber, int pageSize, string Spicility)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationUserViewModel> GetAllPatients()
        {
            var modelList = _unitOfWork.GenericRepository<ApplicationUser>().GetAll().Where(x => x.IsDoctor == false).ToList();
            return ConverModelToViewModelList(modelList);
        }

        public ApplicationUserViewModel GetUserById(string userId)
        {
            var model = _unitOfWork.GenericRepository<ApplicationUser>().GetById(userId);
            return new ApplicationUserViewModel(model);
        }

        public void UpdateUser(ApplicationUserViewModel user)
        {
            var model = _unitOfWork.GenericRepository<ApplicationUser>().GetById(user.Id);
            model.Name = user.Name;
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.Address = user.City;
            model.Gender = user.Gender;
            model.IsDoctor = user.IsDoctor;
            model.Specialist = user.Specialist;

            _unitOfWork.GenericRepository<ApplicationUser>().Update(model);
            _unitOfWork.Save();
        }

        public async Task CreateUserAsync(ApplicationUserViewModel user)
        {
            var newUser = new ApplicationUser
            {
                UserName = user.Email,
                Email = user.Email,
                Name = user.Name,
                Address = user.City,
                Gender = user.Gender,
                IsDoctor = user.IsDoctor,
                Specialist = user.Specialist
            };

            var result = await _userManager.CreateAsync(newUser, user.Password ?? "DefaultPassword123!");
            if (result.Succeeded)
            {
                if (user.IsDoctor)
                {
                    await _userManager.AddToRoleAsync(newUser, WebSiteRoles.WebSite_Doctor);
                }
                else
                {
                    await _userManager.AddToRoleAsync(newUser, WebSiteRoles.WebSite_Patient);
                }
            }
        }

        public void DeleteUser(string userId)
        {
            var model = _unitOfWork.GenericRepository<ApplicationUser>().GetById(userId);
            if (model != null)
            {
                _unitOfWork.GenericRepository<ApplicationUser>().Delete(model);
                _unitOfWork.Save();
            }
        }
    }
}
