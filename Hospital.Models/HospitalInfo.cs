using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class HospitalInfo
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

        public ICollection<Room> Rooms { get; set; }
        public ICollection<Contact> Contacts { get; set; }
    }
}
