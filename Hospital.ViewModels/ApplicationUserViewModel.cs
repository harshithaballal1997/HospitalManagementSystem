using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class ApplicationUserViewModel
    {
        
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }
        public Gender Gender { get; set; }
        public bool IsDoctor { get; set; }
        public string Specialist { get; set; } 

        public ApplicationUserViewModel()
        {

        }
        public ApplicationUserViewModel(ApplicationUser user)
        {
            Name = user.Name;
            Email = user.Email;
            UserName = user.UserName;
            City = user.Address;
            Gender = user.Gender;
            IsDoctor = user.IsDoctor;
            Specialist = user.Specialist;
            List<ApplicationUser> users = new List<ApplicationUser>();
        }
        public ApplicationUser ConvertViewModelToModel(ApplicationUserViewModel user)
        {
            return new ApplicationUser
            {
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Address = user.City,
                Gender = user.Gender,
                IsDoctor = user.IsDoctor,
                Specialist = user.Specialist
            };
        }

        public List<ApplicationUser> Doctors { get; set; } = new List<ApplicationUser>();
    }
}
