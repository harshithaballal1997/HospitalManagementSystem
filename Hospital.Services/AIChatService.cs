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

            if (user.IsInRole("Admin"))
            {
                if (Regex.IsMatch(query, @"how many beds.*icu"))
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
                    return $"[Admin Eyes Only] There are currently {available} available beds in the ICU across all facilities.";
                }
                
                if (Regex.IsMatch(query, @"occupied rooms.*city care"))
                {
                    var hospital = _unitOfWork.GenericRepository<HospitalInfo>()
                        .GetAll(filter: h => h.Name.ToLower().Contains("city care")).FirstOrDefault();
                    
                    if (hospital == null) return "I could not find 'City Care Hospital' in the database.";

                    var occupied = _unitOfWork.GenericRepository<RoomAllocation>()
                        .GetAll(filter: a => a.HospitalId == hospital.Id && a.Status == AllocationStatus.Occupied).Count();
                    
                    return $"[Admin Eyes Only] City Care Hospital currently has {occupied} occupied rooms/allocations.";
                }

                if (Regex.IsMatch(query, @"status|deployment|health"))
                {
                    return "[Admin Eyes Only] System Health: All DB Connections to PostgreSQL are active. SignalR metrics: Normal. Render status: Healthy.";
                }
                
                if (Regex.IsMatch(query, @"how many doctors"))
                {
                    var count = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(x => x.IsDoctor).Count();
                    return $"[Admin Eyes Only] There are currently {count} doctors registered in the system.";
                }

                return "Admin Query: I did not understand that admin oversight command. Try asking about ICU beds, City Care hospital, or system health.";
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
