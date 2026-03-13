using Hospital.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class HospitalInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Country { get; set; }

        // Contact Information
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? WebsiteUrl { get; set; }

        // Details
        public string? OperatingHours { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? Services { get; set; }

        // Location
        public string? StreetAddress { get; set; }

        // Branding
        public string? LogoUrl { get; set; }
        public IFormFile? LogoFile { get; set; }

        public HospitalInfoViewModel()
        {

        }
        public HospitalInfoViewModel(HospitalInfo model)
        {
            Id = model.Id;
            Name = model.Name;
            Type = model.Type;
            City = model.City;
            Pincode = model.Pincode;
            Country = model.Country;
            PhoneNumber = model.PhoneNumber;
            EmailAddress = model.EmailAddress;
            WebsiteUrl = model.WebsiteUrl;
            OperatingHours = model.OperatingHours;
            RegistrationNumber = model.RegistrationNumber;
            Services = model.Services;
            StreetAddress = model.StreetAddress;
            LogoUrl = model.LogoUrl;
        }
        public HospitalInfo ConvertViewModel(HospitalInfoViewModel model)
        {
            return new HospitalInfo{
                Id = model.Id,
                Name = model.Name,
                Type = model.Type,
                City = model.City,
                Pincode = model.Pincode,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber,
                EmailAddress = model.EmailAddress,
                WebsiteUrl = model.WebsiteUrl,
                OperatingHours = model.OperatingHours,
                RegistrationNumber = model.RegistrationNumber,
                Services = model.Services,
                StreetAddress = model.StreetAddress,
                LogoUrl = model.LogoUrl
            };
        }
    }
}
