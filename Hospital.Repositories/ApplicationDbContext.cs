using Hospital.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Repositories
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<HospitalInfo> HospitalInfos { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineReport> MedicineReports { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<PrescribedMedicine> PrescribedMedicines { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PatientReport> PatientReports { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<TestPrice> TestPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This targets the relationship that creates the FK_PatientReport_AspNetUsers_PatientId
            modelBuilder.Entity<PatientReport>()
                // Assuming 'Patient' is the navigation property to the user
                .HasOne(pr => pr.Patient)
                .WithMany() // Adjust this if you have a collection on the User model
                .HasForeignKey(pr => pr.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // <--- THIS IS THE KEY CHANGE

            // Add other model configurations here...
        }

    }
}


