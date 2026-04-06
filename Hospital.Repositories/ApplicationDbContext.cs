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
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
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
        public DbSet<Bed> Beds { get; set; }
        public DbSet<RoomAllocation> RoomAllocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // RoomAllocation relationships
            modelBuilder.Entity<RoomAllocation>()
                .HasOne(ra => ra.Patient)
                .WithMany()
                .HasForeignKey(ra => ra.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomAllocation>()
                .HasOne(ra => ra.Room)
                .WithMany()
                .HasForeignKey(ra => ra.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomAllocation>()
                .HasOne(ra => ra.Bed)
                .WithMany()
                .HasForeignKey(ra => ra.BedId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RoomAllocation>()
                .HasOne(ra => ra.Hospital)
                .WithMany()
                .HasForeignKey(ra => ra.HospitalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bed relationships
            modelBuilder.Entity<Bed>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Beds)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // PatientReport relationships
            modelBuilder.Entity<PatientReport>()
                .HasOne(pr => pr.Patient)
                .WithMany(p => p.PatientReports)
                .HasForeignKey(pr => pr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientReport>()
                .HasOne(pr => pr.Doctor)
                .WithMany()
                .HasForeignKey(pr => pr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientReport>()
                .HasIndex(pr => pr.PatientId);

            // Lab relationships
            modelBuilder.Entity<Lab>()
                .HasOne(l => l.Patient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Lab>()
                .HasIndex(l => l.LabNumber);

            // Add other model configurations here...
            modelBuilder.Entity<Medicine>()
                .Property(p => p.Cost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payroll>()
                .Property(p => p.HourlySalary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payroll>()
                .Property(p => p.NetSalary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payroll>()
                .Property(p => p.Salary)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TestPrice>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Bill>()
                .Property(p => p.Advance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bill>()
                .Property(p => p.MedicineCharge).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bill>()
                .Property(p => p.OperationCharge).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bill>()
                .Property(p => p.RoomCharge).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bill>()
                .Property(p => p.TotalBill).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payroll>()
                .Property(p => p.BonusSalary).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payroll>()
                .Property(p => p.Compensation).HasColumnType("decimal(18,2)");
        }

    }
}


