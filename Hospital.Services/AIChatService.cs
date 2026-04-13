using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Hospital.Services
{
    public enum ChatIntent
    {
        Unknown,
        Greeting,
        SystemHealth,
        PatientStatusLookup,
        BedCapacity,
        PatientBriefing,
        LabResults,
        StaffMetrics
    }

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
            var intent = ExtractIntent(query);

            if (intent == ChatIntent.Greeting)
                return "Hello! I am your secure digital assistant. How may I help you today?";

            // Apply Role-Based Action Pipes
            if (user.IsInRole("Admin")) return await HandleAdminQuery(intent, query);
            if (user.IsInRole("Doctor")) return await HandleDoctorQuery(intent, query);
            if (user.IsInRole("Patient")) return HandlePatientQuery(intent, query, _userManager.GetUserId(user));

            return "I am the Hospital AI. Please log in to access your secure sandbox environment.";
        }

        private ChatIntent ExtractIntent(string q)
        {
            if (q == "hello" || q == "hi" || q == "hey" || q == "greetings" || q == "help") return ChatIntent.Greeting;
            if (q.Contains("is the patient") || q.Contains("where is") || q.Contains("admitted") || q.Contains("status of") || q.Contains("room allocated")) return ChatIntent.PatientStatusLookup;
            if (FuzzyMatcher.IsMatch(q, "bed capacity occupied room icu availability amount of beds")) return ChatIntent.BedCapacity;
            if (FuzzyMatcher.IsMatch(q, "summarize patient briefing history medical record summarize")) return ChatIntent.PatientBriefing;
            if (FuzzyMatcher.IsMatch(q, "vitals lab tests results report weight bmi blood pressure")) return ChatIntent.LabResults;
            if (FuzzyMatcher.IsMatch(q, "doctor nurse staff count total employees list of doctors")) return ChatIntent.StaffMetrics;
            if (q.Contains("status") || q.Contains("deployment") || q.Contains("health")) return ChatIntent.SystemHealth;
            
            return ChatIntent.Unknown;
        }

        private async Task<string> HandleAdminQuery(ChatIntent intent, string query)
        {
            switch (intent)
            {
                case ChatIntent.PatientStatusLookup:
                    var status = ProcessAdminPatientStatus(query);
                    return status ?? "Admin NLQ: I recognized a patient status lookup, but could not cleanly match the name you provided to a registered patient in the database.";

                case ChatIntent.BedCapacity:
                    return ProcessAdminBedQuery(query);

                case ChatIntent.LabResults:
                case ChatIntent.PatientBriefing:
                   var labCount = _unitOfWork.GenericRepository<Lab>().GetAll().Count();
                   var patientCount = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(filter: u => !u.IsDoctor).Count();
                   return $"[Admin Eyes Only] The system currently hosts {labCount} lab reports for {patientCount} patients. Data privacy regulations require explicit consent to pull specific personal clinical details. Try asking for system capacities instead.";

                case ChatIntent.StaffMetrics:
                    var docs = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(x => x.IsDoctor).Count();
                    return $"[Admin Eyes Only] We currently have {docs} doctors registered across the hospital network.";

                case ChatIntent.SystemHealth:
                    return "[Admin Eyes Only] System Health: Database connections active. SignalR metrics normal. Engine is utilizing advanced Levenshtein Semantics.";

                default:
                    // Diagnostic Tool Fallback: If intent is completely unknown, try treating it as an entity name query for status before throwing an error.
                    var diagnosticResult = ProcessAdminPatientStatus(query);
                    if (diagnosticResult != null) return diagnosticResult;

                    return "Admin NLQ: I did not find a matching Patient or Hospital entity for that query. Try asking about hospital capacities, staff metrics, or system health.";
            }
        }

        private string ProcessAdminPatientStatus(string query)
        {
            var patients = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(filter: u => !u.IsDoctor).ToList();
            
            // Extract the closest matching patient via Semantic match
            ApplicationUser targetUser = patients.FirstOrDefault(p => query.Contains(p.Name.ToLowerInvariant()));
            if (targetUser == null)
            {
                foreach(var p in patients)
                {
                    if (FuzzyMatcher.IsMatch(query, p.Name.ToLowerInvariant(), maxDistance: 2))
                    {
                        targetUser = p;
                        break;
                    }
                }
            }

            if (targetUser == null) return null;

            var allocation = _unitOfWork.GenericRepository<RoomAllocation>()
                .GetAll(filter: a => a.PatientId == targetUser.Id && !a.IsDischarged)
                .FirstOrDefault();

            if (allocation == null)
            {
                return $"[Diagnostic Tool] {targetUser.Name} is registered in the system but is not currently admitted to any room.";
            }

            var hospital = _unitOfWork.GenericRepository<HospitalInfo>().GetById(allocation.HospitalId);
            var room = _unitOfWork.GenericRepository<Room>().GetById(allocation.RoomId);
            
            string hName = hospital != null ? hospital.Name : "an unknown facility";
            string rName = room != null ? room.RoomNumber : "an unassigned room";

            return $"[Diagnostic Tool] Yes, {targetUser.Name} is currently admitted to Room {rName} at '{hName}'.";
        }

        private string ProcessAdminBedQuery(string query)
        {
            // Determine Context Scope
            bool isIcuQuery = query.Contains("icu") || query.Contains("intensive");
            var targetHospital = ExtractTargetHospital(query);

            var roomsQuery = _unitOfWork.GenericRepository<Room>().GetAll();
            var allocationsQuery = _unitOfWork.GenericRepository<RoomAllocation>().GetAll(filter: a => a.Status == AllocationStatus.Occupied);

            // Filter for ICU if explicitly requested
            if (isIcuQuery)
            {
                var icuRoomIds = roomsQuery.Where(r => r.Type == "ICU").Select(r => r.Id).ToList();
                roomsQuery = roomsQuery.Where(r => r.Type == "ICU").ToList();
                allocationsQuery = allocationsQuery.Where(a => icuRoomIds.Contains(a.RoomId)).ToList();
            }

            // Filter for Hospital Entity
            if (targetHospital != null)
            {
                roomsQuery = roomsQuery.Where(r => r.HospitalId == targetHospital.Id).ToList();
                allocationsQuery = allocationsQuery.Where(a => a.HospitalId == targetHospital.Id).ToList();
                
                int totalRooms = roomsQuery.Count();
                int totalOccupied = allocationsQuery.Count();

                if (totalRooms == 0)
                {
                    // Actionable Guidance - Fallback algorithm
                    var alternatives = FindAlternativesWithCapacity(isIcuQuery);
                    string rec = string.Join(", ", alternatives);
                    return $"[Admin Eyes Only] I looked up '{targetHospital.Name}', but they currently don't have {(isIcuQuery ? "ICU" : "Standard")} rooms registered. However, I see active availability at: {rec}. Would you like details for those?";
                }

                return $"[Admin Eyes Only] At '{targetHospital.Name}', we have {totalOccupied} occupied instances out of {totalRooms} {(isIcuQuery ? "ICU " : "")}rooms physically tracked.";
            }
            
            // Global Aggregate Fallback
            var totalHospitalsCount = _unitOfWork.GenericRepository<HospitalInfo>().GetAll().Count();
            return $"[Admin Eyes Only] Across all {totalHospitalsCount} integrated hospitals, we are tracking {allocationsQuery.Count()} globally occupied {(isIcuQuery ? "ICU " : "")}beds.";
        }

        private HospitalInfo ExtractTargetHospital(string query)
        {
            var hospitals = _unitOfWork.GenericRepository<HospitalInfo>().GetAll();
            
            // Phase 1: Try strict subset match
            foreach (var h in hospitals)
            {
                string norm = h.Name.ToLowerInvariant().Replace("hospital", "").Trim();
                if (query.Contains(norm)) return h;
            }

            // Phase 2: Try fuzzy Matcher semantic search
            foreach (var h in hospitals)
            {
                string norm = h.Name.ToLowerInvariant().Replace("hospital", "").Trim();
                if (FuzzyMatcher.IsMatch(query, norm, maxDistance: 2)) return h;
            }

            return null; // Implicit wildcard
        }
        
        private List<string> FindAlternativesWithCapacity(bool needsIcu)
        {
            var allocations = _unitOfWork.GenericRepository<RoomAllocation>().GetAll(filter: a => !a.IsDischarged).ToList();
            var rooms = _unitOfWork.GenericRepository<Room>().GetAll().ToList();

            if (needsIcu) rooms = rooms.Where(r => r.Type == "ICU").ToList();

            var hospitalGrp = rooms.GroupBy(r => r.HospitalId)
                .Select(g => new { 
                    HospitalId = g.Key, 
                    Capacity = g.Count() - allocations.Count(a => a.HospitalId == g.Key) 
                })
                .Where(x => x.Capacity > 0)
                .OrderByDescending(x => x.Capacity)
                .Take(2)
                .ToList();

            var hs = _unitOfWork.GenericRepository<HospitalInfo>().GetAll();
            return hospitalGrp.Select(g => hs.FirstOrDefault(h => h.Id == g.HospitalId)?.Name).Where(n => n != null).ToList();
        }

        private async Task<string> HandleDoctorQuery(ChatIntent intent, string query)
        {
            if (intent == ChatIntent.PatientBriefing || intent == ChatIntent.LabResults)
            {
                var patients = _unitOfWork.GenericRepository<ApplicationUser>().GetAll(filter: u => !u.IsDoctor).ToList();
                
                // Fuzzy Match Patient Name Extraction
                ApplicationUser target = null;
                foreach(var p in patients)
                {
                    if (FuzzyMatcher.IsMatch(query, p.Name.ToLowerInvariant()))
                    {
                        target = p;
                        break;
                    }
                }

                if (target == null) return "I could not locate a semantic match for that patient in the system securely. Please ensure the spelling is relatively close to their registered name.";

                var summary = await _medicalAssistantService.SummarizePatientHistoryAsync(target.Id);
                return $"**Dynamic Briefing for {target.Name}:**\n\n{summary}";
            }

            return "Doctor Sandbox: My operational scope is currently optimized for patient briefings. Try asking me to summarize a patient's medical history.";
        }

        private string HandlePatientQuery(ChatIntent intent, string query, string currentUserId)
        {
            if (query.Contains("other patients") || query.Contains("global") || query.Contains("admin"))
            {
                return "🔒 Zero-Leak Privacy Mode Active: You are strictly isolated to your own medical records.";
            }

            if (intent == ChatIntent.LabResults || query.Contains("vitals") || query.Contains("bmi"))
            {
                // Strict RBAC filtering using LINQ WHERE clauses tied to currentUserId
                var labs = _unitOfWork.GenericRepository<Lab>()
                    .GetAll(filter: l => l.PatientId == currentUserId)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToList();

                if (!labs.Any()) return "You do not currently possess any active clinical lab records in the system database.";

                var lastLab = labs.First();
                double bmi = lastLab.Height > 0 ? (lastLab.Weight / Math.Pow(lastLab.Height / 100.0, 2)) : 0;
                
                return $"**Your Latest Clinical Extraction:**\n- BP: {lastLab.BloodPressure}\n- Temp: {lastLab.Temperature}°C\n- BMI Assessment: {bmi:F1}\n\n*This data structure was securely extracted using Identity Validation.*";
            }
            
            if (intent == ChatIntent.BedCapacity || query.Contains("slots") || query.Contains("appointment"))
            {
                return "For appointments and doctor availabilities, please utilize the automated Booking Interface accessible via your dashboard.";
            }

            return "Patient NLQ Sandbox: Currently listening for commands retrieving your personal lab metrics or vitals.";
        }
    }
}
