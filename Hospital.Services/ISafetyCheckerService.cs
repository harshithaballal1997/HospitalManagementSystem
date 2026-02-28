using Hospital.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface ISafetyCheckerService
    {
        Task<SafetyCheckResult> CheckPrescriptionSafetyAsync(string patientId, string medicineName);
        Task<SafetyCheckResult> AnalyzeTestResultAsync(LabViewModel lab);
    }

    public class SafetyCheckResult
    {
        public bool IsSafe { get; set; }
        public string WarningLevel { get; set; } // "None", "Low", "Moderate", "Critical"
        public string Message { get; set; }
        public List<string> Conflicts { get; set; } = new List<string>();
    }
}
