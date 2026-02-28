using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Utilities;

namespace Hospital.Services
{
    public interface IApplicationUserService
    {
        PagedResult<ApplicationUserViewModel> GetAll(int PageNumber, int pageSize);
        PagedResult<ApplicationUserViewModel> GetAllDoctor(int PageNumber, int pageSize);
        PagedResult<ApplicationUserViewModel> GetAllPatient(int PageNumber, int pageSize);
        PagedResult<ApplicationUserViewModel> SearchDoctor(int PageNumber, int pageSize, string Spicility);
        IEnumerable<ApplicationUserViewModel> GetAllPatients();
        ApplicationUserViewModel GetUserById(string userId);
        void UpdateUser(ApplicationUserViewModel user);
        Task CreateUserAsync(ApplicationUserViewModel user);
        void DeleteUser(string userId);
    }
}
