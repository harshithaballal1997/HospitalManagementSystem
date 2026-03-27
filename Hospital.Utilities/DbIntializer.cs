using Hospital.Models;
using Hospital.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Utilities
{
    public class DbIntializer : IDbIntializer
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbIntializer(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public void Initialize()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Count()>0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            if (!_roleManager.RoleExistsAsync(WebSiteRoles.WebSite_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Patient)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebSite_Doctor)).GetAwaiter().GetResult();
            }

            // Seed Users
            void SeedAdminUser(string email, string password)
            {
                var user = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
                if (user == null)
                {
                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    }, password).GetAwaiter().GetResult();
                    user = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
                }

                if (user != null && !_userManager.IsInRoleAsync(user, WebSiteRoles.WebSite_Admin).GetAwaiter().GetResult())
                {
                    _userManager.AddToRoleAsync(user, WebSiteRoles.WebSite_Admin).GetAwaiter().GetResult();
                }
            }

            SeedAdminUser("harkesh@xyz.com", "Harkesh@123");
            SeedAdminUser("harshitha.ballal1997@gmail.com", "Harshitha@97");
            SeedAdminUser("admin@hospital.com", "Admin@123");

            SeedRoomsAndBeds();
        }

        private void SeedRoomsAndBeds()
        {
            // Skip seeding entirely if rooms already exist (avoids thousands of inserts on restart)
            if (_context.Rooms.Any()) return;

            var hospitals = _context.HospitalInfos.ToList();
            if (!hospitals.Any())
            {
                // Seed at least one hospital if none exists
                var defaultHospitals = new List<HospitalInfo>
                {

                };
                _context.HospitalInfos.AddRange(defaultHospitals);
                _context.SaveChanges();
                hospitals.AddRange(defaultHospitals);
            }

            foreach (var hospital in hospitals)
            {
                var existingRooms = _context.Rooms.Where(r => r.HospitalId == hospital.Id).ToList();

                var requestedRooms = new List<(string Name, RoomType Type, int BedCount)>
                {
                    ("Cardiology", RoomType.General, 20),
                    ("Edu", RoomType.General, 20),
                    ("ICU", RoomType.General, 20),
                    ("Medical-Surgical Units", RoomType.General, 20),
                    ("OR", RoomType.General, 20),
                    ("Labour/Delivery", RoomType.General, 20),
                    ("Double Room 1", RoomType.Double, 2),
                    ("Double Room 2", RoomType.Double, 2),
                    ("Private Room 1", RoomType.Private, 1),
                    ("Private Room 2", RoomType.Private, 1)
                };

                foreach (var roomDef in requestedRooms)
                {
                    if (!existingRooms.Any(r => r.RoomNumber == roomDef.Name))
                    {
                        var newRoom = new Room
                        {
                            RoomNumber = roomDef.Name,
                            RoomType = roomDef.Type,
                            Type = roomDef.Type.ToString(),
                            Status = "Active",
                            HospitalId = hospital.Id
                        };
                        _context.Rooms.Add(newRoom);
                        _context.SaveChanges();

                        // Seed Beds
                        for (int i = 1; i <= roomDef.BedCount; i++)
                        {
                            _context.Beds.Add(new Bed
                            {
                                BedNumber = roomDef.Name + "-B" + i,
                                RoomId = newRoom.Id,
                                IsOccupied = false
                            });
                        }
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}
