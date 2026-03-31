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
            SeedPatientsAndClinicalData();
            SeedDoctors();
        }

        private void SeedRoomsAndBeds()
        {
            // Skip seeding entirely if rooms already exist (avoids thousands of inserts on restart)
            // removed early return

            var hospitals = _context.HospitalInfos.ToList();
            if (hospitals.Count <= 1)
            {
                // Seed at least one hospital if none exists
                                var defaultHospitals = new List<HospitalInfo>
                {
                    new HospitalInfo { Name="Mercy General Hospital", Type="General", City="New York", Pincode="10001", Country="USA", PhoneNumber="+1 (212) 555-0101", EmailAddress="info@mercygeneral.com", WebsiteUrl="https://www.mercygeneral.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100001", StreetAddress="300 East 66th Street", Services="NY 10001" },
                    new HospitalInfo { Name="St. Lukes Medical Center", Type="Teaching", City="Chicago", Pincode="60601", Country="USA", PhoneNumber="+1 (312) 555-0102", EmailAddress="contact@stlukesmed.com", WebsiteUrl="https://www.stlukesmed.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100002", StreetAddress="1725 W Harrison St", Services="IL 60612" },
                    new HospitalInfo { Name="Sunrise Specialty Hospital", Type="Specialty", City="Los Angeles", Pincode="90001", Country="USA", PhoneNumber="+1 (213) 555-0103", EmailAddress="hello@sunrisespecialty.com", WebsiteUrl="https://www.sunrisespecialty.com", OperatingHours="Mon-Fri: 6AM-10PM", RegistrationNumber="REG-100003", StreetAddress="5800 Sunset Blvd", Services="CA 90028" },
                    new HospitalInfo { Name="Valley Rehabilitation Center", Type="Rehabilitation", City="Phoenix", Pincode="85001", Country="USA", PhoneNumber="+1 (602) 555-0104", EmailAddress="support@valleyrehab.com", WebsiteUrl="https://www.valleyrehab.com", OperatingHours="Mon-Sat: 7AM-9PM", RegistrationNumber="REG-100004", StreetAddress="1850 N Central Ave", Services="AZ 85004" },
                    new HospitalInfo { Name="Lakeside General Hospital", Type="General", City="Houston", Pincode="77001", Country="USA", PhoneNumber="+1 (713) 555-0105", EmailAddress="info@lakesidegeneral.com", WebsiteUrl="https://www.lakesidegeneral.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100005", StreetAddress="6550 Fannin St", Services="TX 77030" },
                    new HospitalInfo { Name="Greenfield Specialty Clinic", Type="Specialty", City="Philadelphia", Pincode="19101", Country="USA", PhoneNumber="+1 (215) 555-0106", EmailAddress="info@greenfieldspe.com", WebsiteUrl="https://www.greenfieldspe.com", OperatingHours="Mon-Fri: 8AM-6PM", RegistrationNumber="REG-100006", StreetAddress="212 Broad St", Services="PA 19102" },
                    new HospitalInfo { Name="Pinnacle Teaching Hospital", Type="Teaching", City="San Antonio", Pincode="78201", Country="USA", PhoneNumber="+1 (210) 555-0107", EmailAddress="contact@pinnacleteach.com", WebsiteUrl="https://www.pinnacleteach.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100007", StreetAddress="4502 Medical Dr", Services="TX 78229" },
                    new HospitalInfo { Name="Harbor View Medical Center", Type="General", City="San Diego", Pincode="92101", Country="USA", PhoneNumber="+1 (619) 555-0108", EmailAddress="admin@harborviewmc.com", WebsiteUrl="https://www.harborviewmc.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100008", StreetAddress="140 Arbor Dr", Services="CA 92103" },
                    new HospitalInfo { Name="Mountain Recovery Institute", Type="Rehabilitation", City="Dallas", Pincode="75201", Country="USA", PhoneNumber="+1 (214) 555-0109", EmailAddress="info@mountainrecovery.com", WebsiteUrl="https://www.mountainrecovery.com", OperatingHours="Mon-Sat: 7AM-8PM", RegistrationNumber="REG-100009", StreetAddress="8200 Walnut Hill Ln", Services="TX 75231" },
                    new HospitalInfo { Name="Northside Mental Health", Type="Specialty", City="Jacksonville", Pincode="32201", Country="USA", PhoneNumber="+1 (904) 555-0110", EmailAddress="care@northsidemh.com", WebsiteUrl="https://www.northsidemh.com", OperatingHours="Mon-Fri: 9AM-5PM", RegistrationNumber="REG-100010", StreetAddress="655 W 8th St", Services="FL 32209" },
                    new HospitalInfo { Name="Capital General Hospital", Type="General", City="Austin", Pincode="78701", Country="USA", PhoneNumber="+1 (512) 555-0111", EmailAddress="info@capitalgeneral.com", WebsiteUrl="https://www.capitalgeneral.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100011", StreetAddress="1201 W 38th St", Services="TX 78705" },
                    new HospitalInfo { Name="Westbrook University Hospital", Type="Teaching", City="Columbus", Pincode="43201", Country="USA", PhoneNumber="+1 (614) 555-0112", EmailAddress="inquiries@westbrookuh.com", WebsiteUrl="https://www.westbrookuh.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100012", StreetAddress="410 W 10th Ave", Services="OH 43210" },
                    new HospitalInfo { Name="Childrens Specialty Center", Type="Specialty", City="Charlotte", Pincode="28201", Country="USA", PhoneNumber="+1 (704) 555-0113", EmailAddress="kids@childrensspecialty.com", WebsiteUrl="https://www.childrensspecialty.com", OperatingHours="Mon-Fri: 7AM-9PM", RegistrationNumber="REG-100013", StreetAddress="100 Blythe Blvd", Services="NC 28203" },
                    new HospitalInfo { Name="Bay Area Rehab Hospital", Type="Rehabilitation", City="San Francisco", Pincode="94102", Country="USA", PhoneNumber="+1 (415) 555-0114", EmailAddress="info@bayarearehab.com", WebsiteUrl="https://www.bayarearehab.com", OperatingHours="Mon-Sat: 8AM-6PM", RegistrationNumber="REG-100014", StreetAddress="1001 Potrero Ave", Services="CA 94110" },
                    new HospitalInfo { Name="Eastside Community Hospital", Type="General", City="Indianapolis", Pincode="46201", Country="USA", PhoneNumber="+1 (317) 555-0115", EmailAddress="contact@eastsidech.com", WebsiteUrl="https://www.eastsidech.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100015", StreetAddress="1701 N Senate Ave", Services="IN 46202" },
                    new HospitalInfo { Name="Coastal Oncology Specialty", Type="Specialty", City="Seattle", Pincode="98101", Country="USA", PhoneNumber="+1 (206) 555-0116", EmailAddress="oncology@coastalspec.com", WebsiteUrl="https://www.coastalspec.com", OperatingHours="Mon-Fri: 8AM-5PM", RegistrationNumber="REG-100016", StreetAddress="325 9th Ave", Services="WA 98104" },
                    new HospitalInfo { Name="Metro Teaching Medical", Type="Teaching", City="Denver", Pincode="80201", Country="USA", PhoneNumber="+1 (303) 555-0117", EmailAddress="info@metroteaching.com", WebsiteUrl="https://www.metroteaching.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100017", StreetAddress="777 Bannock St", Services="CO 80204" },
                    new HospitalInfo { Name="Southern Cross Hospital", Type="General", City="Nashville", Pincode="37201", Country="USA", PhoneNumber="+1 (615) 555-0118", EmailAddress="admin@southerncrossh.com", WebsiteUrl="https://www.southerncrossh.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100018", StreetAddress="1161 21st Ave S", Services="TN 37232" },
                    new HospitalInfo { Name="Sunrise Physical Therapy", Type="Rehabilitation", City="Baltimore", Pincode="21201", Country="USA", PhoneNumber="+1 (410) 555-0119", EmailAddress="info@sunrisetherapy.com", WebsiteUrl="https://www.sunrisetherapy.com", OperatingHours="Mon-Sat: 7AM-7PM", RegistrationNumber="REG-100019", StreetAddress="22 S Green St", Services="MD 21201" },
                    new HospitalInfo { Name="Pediatric Specialty Hospital", Type="Specialty", City="Louisville", Pincode="40201", Country="USA", PhoneNumber="+1 (502) 555-0120", EmailAddress="kids@pediatricsh.com", WebsiteUrl="https://www.pediatricsh.com", OperatingHours="Mon-Fri: 7AM-8PM", RegistrationNumber="REG-100020", StreetAddress="200 Abraham Flexner Way", Services="KY 40202" },
                    new HospitalInfo { Name="Riverside General Medical", Type="General", City="Portland", Pincode="97201", Country="USA", PhoneNumber="+1 (503) 555-0121", EmailAddress="info@riversidegm.com", WebsiteUrl="https://www.riversidegm.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100021", StreetAddress="3181 SW Sam Jackson Park Rd", Services="OR 97239" },
                    new HospitalInfo { Name="Central University Clinic", Type="Teaching", City="Las Vegas", Pincode="89101", Country="USA", PhoneNumber="+1 (702) 555-0122", EmailAddress="contact@centraluc.com", WebsiteUrl="https://www.centraluc.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-100022", StreetAddress="1800 W Charleston Blvd", Services="NV 89102" },
                    new HospitalInfo { Name="Atlantic Cancer Center", Type="Specialty", City="Memphis", Pincode="38101", Country="USA", PhoneNumber="+1 (901) 555-0123", EmailAddress="info@atlanticcancer.com", WebsiteUrl="https://www.atlanticcancer.com", OperatingHours="Mon-Fri: 8AM-6PM", RegistrationNumber="REG-101023", StreetAddress="1265 Union Ave", Services="TN 38104" },
                    new HospitalInfo { Name="Parkside Rehabilitation", Type="Rehabilitation", City="Albuquerque", Pincode="87101", Country="USA", PhoneNumber="+1 (505) 555-0124", EmailAddress="care@parksiderehab.com", WebsiteUrl="https://www.parksiderehab.com", OperatingHours="Mon-Sat: 8AM-7PM", RegistrationNumber="REG-101024", StreetAddress="2211 Lomas Blvd NE", Services="NM 87106" },
                    new HospitalInfo { Name="Lakeview General Hospital", Type="General", City="Tucson", Pincode="85701", Country="USA", PhoneNumber="+1 (520) 555-0125", EmailAddress="info@lakeviewgh.com", WebsiteUrl="https://www.lakeviewgh.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101025", StreetAddress="1501 N Campbell Ave", Services="AZ 85724" },
                    new HospitalInfo { Name="Horizon Mental Health Clinic", Type="Specialty", City="Fresno", Pincode="93701", Country="USA", PhoneNumber="+1 (559) 555-0126", EmailAddress="support@horizonmhc.com", WebsiteUrl="https://www.horizonmhc.com", OperatingHours="Mon-Fri: 9AM-5PM", RegistrationNumber="REG-101026", StreetAddress="155 Fresno St", Services="CA 93721" },
                    new HospitalInfo { Name="Pacific Research Hospital", Type="Teaching", City="Sacramento", Pincode="95814", Country="USA", PhoneNumber="+1 (916) 555-0127", EmailAddress="research@pacifichosp.com", WebsiteUrl="https://www.pacifichosp.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101027", StreetAddress="2315 Stockton Blvd", Services="CA 95817" },
                    new HospitalInfo { Name="Uptown General Hospital", Type="General", City="Kansas City", Pincode="64101", Country="USA", PhoneNumber="+1 (816) 555-0128", EmailAddress="admin@uptowngeneral.com", WebsiteUrl="https://www.uptowngeneral.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101028", StreetAddress="2301 Holmes St", Services="MO 64108" },
                    new HospitalInfo { Name="Harmony Recovery Center", Type="Rehabilitation", City="Mesa", Pincode="85201", Country="USA", PhoneNumber="+1 (480) 555-0129", EmailAddress="info@harmonyrecovery.com", WebsiteUrl="https://www.harmonyrecovery.com", OperatingHours="Mon-Sat: 7AM-8PM", RegistrationNumber="REG-101029", StreetAddress="525 N Dobson Rd", Services="AZ 85201" },
                    new HospitalInfo { Name="Sunbelt Specialty Hospital", Type="Specialty", City="Atlanta", Pincode="30301", Country="USA", PhoneNumber="+1 (404) 555-0130", EmailAddress="contact@sunbeltsh.com", WebsiteUrl="https://www.sunbeltsh.com", OperatingHours="Mon-Fri: 7AM-8PM", RegistrationNumber="REG-101030", StreetAddress="550 Peachtree St NE", Services="GA 30308" },
                    new HospitalInfo { Name="Maplewood Community Hospital", Type="General", City="Omaha", Pincode="68101", Country="USA", PhoneNumber="+1 (402) 555-0131", EmailAddress="info@maplewoodch.com", WebsiteUrl="https://www.maplewoodch.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101031", StreetAddress="7500 Mercy Rd", Services="NE 68124" },
                    new HospitalInfo { Name="Grand Valley Teaching Hospital", Type="Teaching", City="Colorado Springs", Pincode="80901", Country="USA", PhoneNumber="+1 (719) 555-0132", EmailAddress="contact@grandvalleyth.com", WebsiteUrl="https://www.grandvalleyth.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101032", StreetAddress="1400 E Boulder St", Services="CO 80909" },
                    new HospitalInfo { Name="Desert Pediatric Center", Type="Specialty", City="Raleigh", Pincode="27601", Country="USA", PhoneNumber="+1 (919) 555-0133", EmailAddress="hello@desertpediatric.com", WebsiteUrl="https://www.desertpediatric.com", OperatingHours="Mon-Fri: 8AM-6PM", RegistrationNumber="REG-101033", StreetAddress="3000 New Bern Ave", Services="NC 27610" },
                    new HospitalInfo { Name="Green Valley Rehab Institute", Type="Rehabilitation", City="Long Beach", Pincode="90801", Country="USA", PhoneNumber="+1 (562) 555-0134", EmailAddress="info@greenvalleyri.com", WebsiteUrl="https://www.greenvalleyri.com", OperatingHours="Mon-Sat: 8AM-8PM", RegistrationNumber="REG-101034", StreetAddress="2776 Pacific Ave", Services="CA 90806" },
                    new HospitalInfo { Name="Bayfront General Hospital", Type="General", City="Minneapolis", Pincode="55401", Country="USA", PhoneNumber="+1 (612) 555-0135", EmailAddress="admin@bayfrontgh.com", WebsiteUrl="https://www.bayfrontgh.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101035", StreetAddress="701 Park Ave", Services="MN 55415" },
                    new HospitalInfo { Name="Eastern Oncology Specialists", Type="Specialty", City="Tampa", Pincode="33601", Country="USA", PhoneNumber="+1 (813) 555-0136", EmailAddress="care@easternoncology.com", WebsiteUrl="https://www.easternoncology.com", OperatingHours="Mon-Fri: 8AM-5PM", RegistrationNumber="REG-101036", StreetAddress="1 Tampa General Cir", Services="FL 33606" },
                    new HospitalInfo { Name="Capital City University Med", Type="Teaching", City="New Orleans", Pincode="70112", Country="USA", PhoneNumber="+1 (504) 555-0137", EmailAddress="contact@capitalcitymed.com", WebsiteUrl="https://www.capitalcitymed.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101037", StreetAddress="2021 Perdido St", Services="LA 70112" },
                    new HospitalInfo { Name="Northgate General Hospital", Type="General", City="Arlington", Pincode="76001", Country="USA", PhoneNumber="+1 (817) 555-0138", EmailAddress="info@northgategh.com", WebsiteUrl="https://www.northgategh.com", OperatingHours="Mon-Sun: 24/7", RegistrationNumber="REG-101038", StreetAddress="800 W Randol Mill Rd", Services="TX 76012" },
                    new HospitalInfo { Name="Clearwater Physical Institute", Type="Rehabilitation", City="Bakersfield", Pincode="93301", Country="USA", PhoneNumber="+1 (661) 555-0139", EmailAddress="info@clearwaterpi.com", WebsiteUrl="https://www.clearwaterpi.com", OperatingHours="Mon-Sat: 7AM-7PM", RegistrationNumber="REG-101039", StreetAddress="2615 Eye St", Services="CA 93301" },
                    new HospitalInfo { Name="Midtown Mental Wellness", Type="Specialty", City="Aurora", Pincode="80010", Country="USA", PhoneNumber="+1 (720) 555-0140", EmailAddress="wellness@midtownmw.com", WebsiteUrl="https://www.midtownmw.com", OperatingHours="Mon-Fri: 9AM-6PM", RegistrationNumber="REG-101040", StreetAddress="1375 E Colfax Ave", Services="CO 80010" }
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
            _context.ChangeTracker.Clear();
        }

        
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
            var patientsToUpdate = _context.Users.OfType<ApplicationUser>().Where(u => u.Name.StartsWith("Test Patient ")).ToList();
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
    }
}
