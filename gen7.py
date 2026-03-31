import re

with open('Hospital.Utilities/DbIntializer.cs', 'r', encoding='utf-8') as f:
    src = f.read()

doctor_seed_method = r"""
        private void SeedDoctors()
        {
            _context.ChangeTracker.Clear();
            var doctorsCount = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult().Count();
            if (doctorsCount >= 40) return; // Already seeded

            var passwordHash = _userManager.FindByEmailAsync("admin@hospital.com").GetAwaiter().GetResult()?.PasswordHash;
            var role = _roleManager.FindByNameAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult();

            string[] firstNamesM = { "Gregory", "Derek", "Preston", "Richard", "Owen", "Jackson", "Mark", "Stephen", "Martin", "Conrad", "Shaun", "Neil", "Marcus", "Levi", "Christopher" };
            string[] firstNamesF = { "Meredith", "Cristina", "Miranda", "Lexie", "Arizona", "April", "Amelia", "Maggie", "Jo", "Teddy", "Callie", "Addison", "Nicole", "Claire", "Alison" };
            string[] lastNames = { "House", "Shepherd", "Burke", "Webber", "Hunt", "Avery", "Sloan", "Strange", "Melendez", "Hawkins", "Murphy", "Melendez", "Andrews", "Benson", "Turk" };
            string[] specializations = { "Cardiology", "Neurology", "General Surgery", "Pediatrics", "Orthopedics", "Oncology", "Psychiatry", "Emergency Medicine", "Radiology", "Anesthesiology" };

            var newDoctors = new List<ApplicationUser>();
            var userRoles = new List<IdentityUserRole<string>>();
            
            var hospitals = _context.HospitalInfos.ToList();
            if (!hospitals.Any()) return;

            int docIndex = 1;
            int hIndex = 0;

            // Generate ~80 doctors
            for (int i = 0; i < 80; i++)
            {
                string fname = (i % 2 == 0) ? firstNamesM[i / 2 % 15] : firstNamesF[i / 2 % 15];
                string lname = lastNames[(i * 3) % 15];
                string spec = specializations[i % 10];
                var docId = Guid.NewGuid().ToString();

                // Assign to hospital mapping logic:
                // Constraints: All hospitals need at least one doctor. Least one needs 10.
                // Hospital 0 gets 10 doctors (docIndex 1 to 10)
                // Rest of the 70 doctors go to the remaining 39 hospitals (1-2 each)
                HospitalInfo assignedHosp = hospitals[0];
                if (docIndex > 10)
                {
                    hIndex++;
                    assignedHosp = hospitals[hIndex % hospitals.Count];
                }

                var d = new ApplicationUser
                {
                    Id = docId,
                    UserName = $"dr.{fname.ToLower()}.{lname.ToLower()}{docIndex}@hospital.com",
                    NormalizedUserName = $"DR.{fname.ToUpper()}.{lname.ToUpper()}{docIndex}@HOSPITAL.COM",
                    Email = $"dr.{fname.ToLower()}.{lname.ToLower()}{docIndex}@hospital.com",
                    NormalizedEmail = $"DR.{fname.ToUpper()}.{lname.ToUpper()}{docIndex}@HOSPITAL.COM",
                    EmailConfirmed = true,
                    PasswordHash = passwordHash,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Name = $"Dr. {fname} {lname}",
                    Gender = (i % 2 == 0) ? Gender.Male : Gender.Female,
                    Nationality = "USA",
                    Address = $"{assignedHosp.Name}, {assignedHosp.City}", // Maps to Location in UI
                    DOB = DateTime.UtcNow.AddYears(-35 - (i % 20)),
                    IsDoctor = true,
                    Specialist = spec
                };
                newDoctors.Add(d);
                userRoles.Add(new IdentityUserRole<string> { RoleId = role.Id, UserId = docId });
                docIndex++;
            }

            _context.Users.AddRange(newDoctors);
            _context.UserRoles.AddRange(userRoles);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }
"""

if 'SeedDoctors();' not in src:
    src = src.replace('SeedPatientsAndClinicalData();', 'SeedPatientsAndClinicalData();\n            SeedDoctors();')

if 'private void SeedDoctors()' not in src:
    idx = src.rfind('    }')
    src = src[:idx] + doctor_seed_method + src[idx:]

with open('Hospital.Utilities/DbIntializer.cs', 'w', encoding='utf-8') as f:
    f.write(src)
print("Patch Applied")
