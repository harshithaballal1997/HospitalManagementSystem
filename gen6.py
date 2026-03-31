import re

with open('Hospital.Utilities/DbIntializer.cs', 'r', encoding='utf-8') as f:
    src = f.read()

# I will replace the inside of SeedPatientsAndClinicalData() completely to handle BOTH initial seeding and LIVE data updates.

csharp_patch = r"""
        private void SeedPatientsAndClinicalData()
        {
            _context.ChangeTracker.Clear();
            var adminId = _userManager.FindByEmailAsync("admin@hospital.com").GetAwaiter().GetResult()?.Id;
            var doctorId = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Doctor).GetAwaiter().GetResult().FirstOrDefault()?.Id;
            if (doctorId == null) doctorId = adminId;

            var patientsCount = _userManager.GetUsersInRoleAsync(WebSiteRoles.WebSite_Patient).GetAwaiter().GetResult().Count();
            
            string[] firstNamesM = { "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Jerry" };
            string[] firstNamesF = { "Mary", "Patricia", "Linda", "Barbara", "Elizabeth", "Jennifer", "Maria", "Susan", "Margaret", "Dorothy", "Lisa", "Nancy", "Karen", "Betty", "Helen", "Sandra", "Donna", "Carol", "Ruth", "Sharon", "Michelle", "Laura", "Sarah", "Kimberly", "Deborah", "Jessica", "Shirley", "Cynthia", "Angela", "Melissa", "Brenda", "Amy", "Anna", "Rebecca", "Virginia", "Kathleen", "Pamela", "Martha", "Debra", "Amanda", "Stephanie", "Carolyn", "Christine", "Marie", "Janet", "Catherine", "Frances", "Ann", "Joyce", "Diane" };
            string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts" };

            var passwordHash = _userManager.FindByEmailAsync("admin@hospital.com").GetAwaiter().GetResult()?.PasswordHash;
            var role = _roleManager.FindByNameAsync(WebSiteRoles.WebSite_Patient).GetAwaiter().GetResult();

            if (patientsCount < 100)
            {
                var newPatients = new List<ApplicationUser>();
                var userRoles = new List<IdentityUserRole<string>>();

                for (int i = 1; i <= 100; i++)
                {
                    string fname = (i % 2 == 0) ? firstNamesM[i / 2 % 50] : firstNamesF[i / 2 % 50];
                    string lname = lastNames[(i * 3) % 50];
                    var patientId = Guid.NewGuid().ToString();
                    var p = new ApplicationUser
                    {
                        Id = patientId,
                        UserName = $"{fname.ToLower()}@hospital.com",
                        NormalizedUserName = $"{fname.ToUpper()}@HOSPITAL.COM",
                        Email = $"{fname.ToLower()}@hospital.com",
                        NormalizedEmail = $"{fname.ToUpper()}@HOSPITAL.COM",
                        EmailConfirmed = true,
                        PasswordHash = passwordHash,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Name = $"{fname} {lname}",
                        Gender = (i % 2 == 0) ? Gender.Male : Gender.Female,
                        Nationality = "USA",
                        Address = $"{i}00 Health Ave, City",
                        DOB = DateTime.UtcNow.AddYears(-20 - (i % 50)),
                        IsDoctor = false
                    };
                    newPatients.Add(p);
                    userRoles.Add(new IdentityUserRole<string> { RoleId = role.Id, UserId = patientId });
                }

                _context.Users.AddRange(newPatients);
                _context.UserRoles.AddRange(userRoles);
                _context.SaveChanges();

                var hospitalsList = _context.HospitalInfos.Include(h => h.Rooms).ThenInclude(r => r.Beds).ToList();
                var seriousPatients = newPatients.Take(30).ToList();
                var allocations = new List<RoomAllocation>();
                var labs = new List<Lab>();

                int hIdx = 0;
                foreach (var sp in seriousPatients)
                {
                    var hosp = hospitalsList[hIdx % hospitalsList.Count];
                    var bed = hosp.Rooms.SelectMany(r => r.Beds).FirstOrDefault(b => !b.IsOccupied);
                    if (bed != null)
                    {
                        bed.IsOccupied = true;
                        allocations.Add(new RoomAllocation
                        {
                            PatientId = sp.Id,
                            RoomId = bed.RoomId,
                            BedId = bed.Id,
                            HospitalId = hosp.Id,
                            IsDischarged = false,
                            AllocationDate = DateTime.UtcNow.AddDays(-5),
                            Status = AllocationStatus.Occupied
                        });
                    }
                    hIdx++;

                    double[] values = { 75.5, 85.0, 95.5, 110.5, 125.0 };
                    string[] statuses = { "Normal", "Slightly Elevated", "Risky", "High Risk", "Danger / Critical" };

                    for (int m = 0; m < 5; m++)
                    {
                        labs.Add(new Lab
                        {
                            PatientId = sp.Id,
                            LabNumber = "LAB-" + DateTime.UtcNow.Ticks.ToString().Substring(8) + m,
                            TestType = "Metabolic Panel",
                            TestCode = "CMP-01",
                            Weight = 70 + (m % 3),
                            Height = 175,
                            BloodPressure = 120 + (m * 5),
                            Temperature = 37 + (m > 2 ? 1 : 0),
                            TestValue = values[m],
                            TestResults = statuses[m],
                            CreatedAt = DateTime.UtcNow.AddDays(-5 + m)
                        });
                    }
                }
                _context.RoomAllocations.AddRange(allocations);
                _context.Labs.AddRange(labs);
                _context.SaveChanges();
            }

            // LIVE UPDATE PHASE (For existing "Test Patient N" records previously seeded)
            var patientsToUpdate = _context.Users.Where(u => u.Name.StartsWith("Test Patient ")).ToList();
            if (patientsToUpdate.Any())
            {
                int index = 0;
                foreach(var p in patientsToUpdate)
                {
                    string fname = (index % 2 == 0) ? firstNamesM[index / 2 % 50] : firstNamesF[index / 2 % 50];
                    string lname = lastNames[(index * 3) % 50];
                    p.Name = $"{fname} {lname}";
                    p.Email = $"{fname.ToLower()}{index}@hospital.com"; // Add index to avoid duplicate emails
                    p.UserName = p.Email;
                    p.NormalizedEmail = p.Email.ToUpper();
                    p.NormalizedUserName = p.Email.ToUpper();
                    p.Gender = (index % 2 == 0) ? Gender.Male : Gender.Female;
                    index++;
                }
                _context.SaveChanges();
            }

            // Scatter existing allocations across hospitals if they are all clustered
            var allocationsToUpdate = _context.RoomAllocations.Include(a => a.Bed).ThenInclude(b => b.Room).ToList();
            if (allocationsToUpdate.Any() && allocationsToUpdate.All(a => a.HospitalId == allocationsToUpdate.First().HospitalId))
            {
                var hospitalsWithBeds = _context.HospitalInfos.Include(h => h.Rooms).ThenInclude(r => r.Beds).ToList();
                int hIndex = 0;
                foreach(var alloc in allocationsToUpdate)
                {
                    var hosp = hospitalsWithBeds[hIndex % hospitalsWithBeds.Count];
                    if (hosp.Id != alloc.HospitalId)
                    {
                        var newBed = hosp.Rooms.SelectMany(r => r.Beds).FirstOrDefault(b => !b.IsOccupied && b.Id != alloc.BedId);
                        if (newBed != null)
                        {
                            if (alloc.Bed != null) alloc.Bed.IsOccupied = false;
                            alloc.BedId = newBed.Id;
                            alloc.RoomId = newBed.RoomId;
                            alloc.HospitalId = hosp.Id;
                            newBed.IsOccupied = true;
                        }
                    }
                    hIndex++;
                }
                _context.SaveChanges();
            }
        }
"""

src = re.sub(r'private void SeedPatientsAndClinicalData\(\)\s*\{.*?\n        \}', csharp_patch, src, flags=re.DOTALL)

with open('Hospital.Utilities/DbIntializer.cs', 'w', encoding='utf-8') as f:
    f.write(src)
print("Patch Applied")
