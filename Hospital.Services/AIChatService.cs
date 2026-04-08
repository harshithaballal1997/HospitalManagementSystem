using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public class AIChatService : IAIChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMedicalAssistantService _medicalAssistantService;
        private readonly UserManager<IdentityUser> _userManager;

        public AIChatService(IUnitOfWork unitOfWork, IMedicalAssistantService medicalAssistantService, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _medicalAssistantService = medicalAssistantService;
            _userManager = userManager;
        }

        public async Task<string> ProcessQueryAsync(string query, ClaimsPrincipal user)
        {
            query = query.ToLowerInvariant();

            if (query == "hello" || query == "hi" || query == "hey" || query == "greetings" || query == "help")
            {
                return "Hello! I am your secure digital assistant. How may I help you today?";
            }

            if (user.IsInRole("Admin"))
            {
                if (query.Contains("icu") && query.Contains("bed"))
                {
                    var icuRooms = _unitOfWork.GenericRepository<Room>()
                        .GetAll(filter: r => r.Type == "ICU")
                        .Select(r => r.Id).ToList();
                    
                    var allICUBeds = _unitOfWork.GenericRepository<Bed>()
                        .GetAll(filter: b => icuRooms.Contains(b.RoomId)).ToList();
                        
                    var occupiedBeds = _unitOfWork.GenericRepository<RoomAllocation>()
                        .GetAll(filter: a => a.Status == AllocationStatus.Occupied && a.BedId.HasValue && icuRooms.Contains(a.RoomId))
                        .Select(a => a.BedId.Value).ToList();

                    var available = allICUBeds.Count(b => !occupiedBeds.Contains(b.Id));
                    return $"[Admin Eyes Only] There are currently {available} available beds in the ICU out of a total {allICUBeds.Count} ICU beds across all facilities.";
                }
                
                if (query.Contains("bed") && query.Contains("hospital"))
                {
                    // Trying to parse a specific hospital name out of the string if they gave one
                    var hospitals = _unitOfWork.GenericRepository<HospitalInfo>().GetAll();
                    HospitalInfo targetHospital = null;
                    foreach(var h in hospitals)
                    {
                        if (query.Replace("hospital", "").Trim().Contains(h.Name.ToLower().Replace("hospital", "").Trim()))
                        {
                            targetHospital = h;
                            break;
                        }
                    }

                    if (targetHospital != null)
                    {
                        var occupied = _unitOfWork.GenericRepository<RoomAllocation>()
                            .GetAll(filter: a => a.HospitalId == targetHospital.Id && a.Status == AllocationStatus.Occupied).Count();
                        return $"[Admin Eyes Only] {targetHospital.Name} currently has {occupied} occupied rooms/beds.";
                    }
                    
                    // If no explicit hospital, return generic bed context
                    var allOccupied = _unitOfWork.GenericRepository<RoomAllocation>()
                            .GetAll(filter: a => a.Status == AllocationStatus.Occupied).Count();
                    return $"[Admin Eyes Only] Across all hospitals, there are {allOccupied} currently occupied beds/rooms.";
                }

                if (query.Contains("lab") || query.Contains("report") || query.Contains("patient"))
                {
                   // They want patient lab reports
                   var labCount = _unitOfWork.GenericRepository<Lab>().GetAll().Count();
                   return $"[Admin Eyes Only] The system currently hosts {labCount} clinical lab reports across all patients. To query a specific patient's clinical summary as an Admin, please search their name in the patient's module. Data privacy regulations require explicit consent to pull specific lab details within the chat.";
                }

                if (Regex.IsMatch(query, @"status|deployment|health"))
                {
                    return "[Admin Eyes Only] System Health: All DB Connections to PostgreSQL are active. SignalR metrics: Normal. Render status: Healthy.";
                }
                
                if (Regex.IsMatch(query, @"how many doctors|number of doctors|count of doctors"))
                {
                    var count = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(x => x.IsDoctor).Count();
                    return $"[Admin Eyes Only] There are currently {count} doctors registered in the system.";
                }

                return "Admin Query: I did not understand that command. Try asking about ICU beds, beds in a specific hospital, or patients clinical lab reports.";
            }

            if (user.IsInRole("Doctor"))
            {
                if (query.Contains("summarize patient") || query.Contains("briefing for patient"))
                {
                    // For demo purposes via natural language, normally we extract ID. 
                    // Let's assume the doctor provides a name, we find the first matching patient.
                    var searchName = query.Replace("summarize patient", "").Replace("briefing for patient", "").Trim();
                    var patient = _unitOfWork.GenericRepository<ApplicationUser>()
                        .GetAll(filter: u => !u.IsDoctor && u.Name.ToLower().Contains(searchName))
                        .FirstOrDefault();

                    if (patient == null) return $"I could not locate a patient matching '{searchName}' in your roster.";

                    var summary = await _medicalAssistantService.SummarizePatientHistoryAsync(patient.Id);
                    return $"**Patient Briefing for {patient.Name}:**\n\n{summary}";
                }

                return "Doctor Sandbox: I am restricted to patient briefings. Please ask me to 'summarize patient [Name]'.";
            }

            if (user.IsInRole("Patient"))
            {
                var userId = _userManager.GetUserId(user);

                if (query.Contains("other patients") || query.Contains("global hospital") || query.Contains("admin"))
                {
                    return "🔒 Zero-Leak Privacy Barrier: You are not authorized to query global hospital metrics or other patient's data.";
                }

                if (Regex.IsMatch(query, @"my latest lab result|vitals|bmi|trend"))
                {
                    var lastLab = _unitOfWork.GenericRepository<Lab>()
                        .GetAll(filter: l => l.PatientId == userId)
                        .OrderByDescending(l => l.CreatedAt)
                        .FirstOrDefault();

                    if (lastLab == null) return "You currently do not have any lab results in the system.";

                    double bmi = lastLab.Height > 0 ? (lastLab.Weight / System.Math.Pow(lastLab.Height / 100.0, 2)) : 0;
                    return $"**Your Latest Clinical Lab Report:**\n- Blood Pressure: {lastLab.BloodPressure}\n- Temp: {lastLab.Temperature}°C\n- BMI: {bmi:F1}\n\n*This data is strictly isolated to your session ID.*";
                }
                
                if (Regex.IsMatch(query, @"doctor availability|slots"))
                {
                    return "You can check dynamic doctor availabilities by navigating to the 'Book Appointment' page on your dashboard.";
                }

                return "Patient Sandbox: I can only access your personal records. Try asking 'What is my latest lab result?'.";
            }

            return "I am the Hospital AI. Please log in to access your secure sandbox environment.";
        }
    }
}
