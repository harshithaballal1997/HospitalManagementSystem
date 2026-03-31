import re

with open('Hospital.Utilities/DbIntializer.cs', 'r', encoding='utf-8') as f:
    src = f.read()

doctor_seed_method = r"""
        private void SeedDoctors()
        {
            _context.ChangeTracker.Clear();
            var doctorsCount = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult().Count();
            
            // Re-seed if count is low or if we want to ensure the 10-12 distribution
            if (doctorsCount < 200)
            {
                var existingDocs = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult();
                foreach (var doc in existingDocs)
                {
                    _userManager.DeleteAsync(doc).GetAwaiter().GetResult();
                }
                _context.SaveChanges();
            }
            else
            {
                return; // Already densely seeded
            }

            var passwordHash = _userManager.FindByEmailAsync("admin@hospital.com").GetAwaiter().GetResult()?.PasswordHash;
            var role = _roleManager.FindByNameAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult();

            string[] firstNamesM = { "Gregory", "Derek", "Preston", "Richard", "Owen", "Jackson", "Mark", "Stephen", "Martin", "Conrad", "Shaun", "Neil", "Marcus", "Levi", "Christopher", "James", "John", "Robert", "Michael", "William", "David" };
            string[] firstNamesF = { "Meredith", "Cristina", "Miranda", "Lexie", "Arizona", "April", "Amelia", "Maggie", "Jo", "Teddy", "Callie", "Addison", "Nicole", "Claire", "Alison", "Mary", "Patricia", "Linda", "Barbara", "Elizabeth" };
            string[] lastNames = { "House", "Shepherd", "Burke", "Webber", "Hunt", "Avery", "Sloan", "Strange", "Melendez", "Hawkins", "Murphy", "Andrews", "Benson", "Turk", "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller" };
            string[] specializations = { "Cardiology", "Neurology", "General Surgery", "Pediatrics", "Orthopedics", "Oncology", "Psychiatry", "Emergency Medicine", "Radiology", "Anesthesiology" };

            var newDoctors = new List<ApplicationUser>();
            var userRoles = new List<IdentityUserRole<string>>();
            
            var hospitals = _context.HospitalInfos.ToList();
            if (!hospitals.Any()) return;

            Random rand = new Random(999);
            int docTotalIndex = 1;

            foreach (var hosp in hospitals)
            {
                // Increase density to 10-12 per hospital
                int numDocs = rand.Next(10, 13); 
                for (int d = 0; d < numDocs; d++)
                {
                    bool isMale = rand.Next(2) == 0;
                    string fname = isMale ? firstNamesM[rand.Next(firstNamesM.Length)] : firstNamesF[rand.Next(firstNamesF.Length)];
                    string lname = lastNames[rand.Next(lastNames.Length)];
                    string spec = specializations[rand.Next(specializations.Length)];
                    var docId = Guid.NewGuid().ToString();

                    var doctor = new ApplicationUser
                    {
                        Id = docId,
                        UserName = $"dr.{fname.ToLower()}.{lname.ToLower()}{docTotalIndex}@hospital.com",
                        NormalizedUserName = $"DR.{fname.ToUpper()}.{lname.ToUpper()}{docTotalIndex}@HOSPITAL.COM",
                        Email = $"dr.{fname.ToLower()}.{lname.ToLower()}{docTotalIndex}@hospital.com",
                        NormalizedEmail = $"DR.{fname.ToUpper()}.{lname.ToUpper()}{docTotalIndex}@HOSPITAL.COM",
                        EmailConfirmed = true,
                        PasswordHash = passwordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Name = $"Dr. {fname} {lname}",
                        Gender = isMale ? Gender.Male : Gender.Female,
                        Nationality = "USA",
                        Address = $"{hosp.Name}, {hosp.City}", 
                        DOB = DateTime.UtcNow.AddYears(-30 - rand.Next(30)),
                        IsDoctor = true,
                        Specialist = spec
                    };
                    newDoctors.Add(doctor);
                    userRoles.Add(new IdentityUserRole<string> { RoleId = role.Id, UserId = docId });
                    docTotalIndex++;
                }
            }

            _context.Users.AddRange(newDoctors);
            _context.UserRoles.AddRange(userRoles);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }
"""

src = re.sub(r'private void SeedDoctors\(\)\s*\{.*?\n        \}', doctor_seed_method, src, flags=re.DOTALL)

with open('Hospital.Utilities/DbIntializer.cs', 'w', encoding='utf-8') as f:
    f.write(src)
print("Patch 9 Applied")
