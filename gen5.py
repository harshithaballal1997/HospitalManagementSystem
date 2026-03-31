import json

code = """
        private void SeedPatientsAndClinicalData()
        {
            var adminId = _userManager.FindByEmailAsync("admin@hospital.com").GetAwaiter().GetResult()?.Id;
            var doctorId = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult().FirstOrDefault()?.Id;
            
            // To ensure we have a doctor, get one or just use adminId if none
            if (doctorId == null) doctorId = adminId;

            var patientsCount = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Patient).GetAwaiter().GetResult().Count();
            if (patientsCount >= 100) return;

            var passwordHash = _userManager.FindByEmailAsync("admin@hospital.com").GetAwaiter().GetResult()?.PasswordHash;

            var role = _roleManager.FindByNameAsync(WebSiteRoles.WebSite_Patient).GetAwaiter().GetResult();
            var newPatients = new List<ApplicationUser>();
            var userRoles = new List<IdentityUserRole<string>>();

            Random rand = new Random(12345);
            for (int i = 1; i <= 100; i++)
            {
                var patientId = Guid.NewGuid().ToString();
                var p = new ApplicationUser
                {
                    Id = patientId,
                    UserName = $"patient{i}@hospital.com",
                    NormalizedUserName = $"PATIENT{i}@HOSPITAL.COM",
                    Email = $"patient{i}@hospital.com",
                    NormalizedEmail = $"PATIENT{i}@HOSPITAL.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHash,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Name = $"Test Patient {i}",
                    Gender = (i % 2 == 0) ? Gender.Female : Gender.Male,
                    Nationality = "USA",
                    Address = $"{i}00 Health Ave, City",
                    DOB = DateTime.Now.AddYears(-20 - (i % 50)),
                    IsDoctor = false
                };
                newPatients.Add(p);
                userRoles.Add(new IdentityUserRole<string> { RoleId = role.Id, UserId = patientId });
            }

            _context.Users.AddRange(newPatients);
            _context.UserRoles.AddRange(userRoles);
            _context.SaveChanges();

            // Allocate 30 patients as 'serious'
            var beds = _context.Beds.Where(b => !b.IsOccupied).Take(30).ToList();
            if (beds.Count < 30) return; // not enough beds

            var seriousPatients = newPatients.Take(30).ToList();
            var allocations = new List<RoomAllocation>();
            var labs = new List<Lab>();

            int bIdx = 0;
            foreach (var sp in seriousPatients)
            {
                var bed = beds[bIdx++];
                bed.IsOccupied = true;
                allocations.Add(new RoomAllocation
                {
                    PatientId = sp.Id,
                    RoomId = bed.RoomId,
                    BedId = bed.Id,
                    BedNumber = bed.BedNumber,
                    IsAdmitted = true,
                    DateAdmitted = DateTime.Now.AddDays(-5),
                    Status = "Critical"
                });

                // 5 Lab Results progressing from Normal to Danger
                // Normal (60-80), Risky (80-100), Danger (100+)
                double[] values = { 75.5, 85.0, 95.5, 110.5, 125.0 };
                string[] statuses = { "Normal", "Slightly Elevated", "Risky", "High Risk", "Danger / Critical" };

                for (int m = 0; m < 5; m++)
                {
                    labs.Add(new Lab
                    {
                        PatientId = sp.Id,
                        LabNumber = "LAB-" + DateTime.Now.Ticks.ToString().Substring(8) + m,
                        TestType = "Metabolic Panel",
                        TestCode = "CMP-01",
                        Weight = 70 + (m % 3),
                        Height = 175,
                        BloodPressure = 120 + (m * 5), // BP rising
                        Temperature = 37 + (m > 2 ? 1 : 0),
                        TestValue = values[m],
                        TestResults = statuses[m],
                        CreatedAt = DateTime.Now.AddDays(-5 + m)
                    });
                }
            }

            _context.RoomAllocations.AddRange(allocations);
            _context.Labs.AddRange(labs);
            _context.SaveChanges();
        }
"""

with open('Hospital.Utilities/DbIntializer.cs', 'r') as f:
    src = f.read()

# Insert method call and method body
import re
if 'SeedPatientsAndClinicalData();' not in src:
    src = src.replace('SeedRoomsAndBeds();', 'SeedRoomsAndBeds();\n            SeedPatientsAndClinicalData();')

if 'private void SeedPatientsAndClinicalData()' not in src:
    # insert before last 2 braces
    idx = src.rfind('    }')
    src = src[:idx] + code + src[idx:]

with open('Hospital.Utilities/DbIntializer.cs', 'w', encoding='utf-8') as f:
    f.write(src)
print("Patch applied to DbIntializer")
