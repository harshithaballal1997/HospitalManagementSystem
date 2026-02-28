using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public class SafetyCheckerService : ISafetyCheckerService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Mock database of drug-allergy and drug-drug interactions for the AI prototype
        private readonly Dictionary<string, List<string>> _knownConflicts = new Dictionary<string, List<string>>
        {
            { "Penicillin", new List<string> { "Amoxicillin", "Ampicillin" } },
            { "Aspirin", new List<string> { "Warfarin", "Ibuprofen" } },
            { "Statins", new List<string> { "Grapefruit Juice" } }
        };

        public SafetyCheckerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SafetyCheckResult> CheckPrescriptionSafetyAsync(string patientId, string medicineName)
        {
            var result = new SafetyCheckResult { IsSafe = true, WarningLevel = "None" };

            // 1. Check existing prescriptions for drug-drug interactions
            var currentMedications = _unitOfWork.GenericRepository<PrescribedMedicine>()
                .GetAll(filter: pm => pm.PatientReport.PatientId == patientId)
                .Select(pm => pm.Medicine.Name)
                .ToList();

            foreach (var med in currentMedications)
            {
                if (_knownConflicts.ContainsKey(med) && _knownConflicts[med].Contains(medicineName))
                {
                    result.IsSafe = false;
                    result.WarningLevel = "Critical";
                    result.Conflicts.Add($"Interaction with current medication: {med}");
                }
            }

            // 2. Check patient record for allergy mentions (SIMULATED AI analysis)
            var patientNotes = _unitOfWork.GenericRepository<PatientReport>()
                .GetAll(filter: pr => pr.PatientId == patientId)
                .Select(pr => pr.Diagnose + " " + pr.MedicineName)
                .ToList();

            string combinedNotes = string.Join(" ", patientNotes).ToLower();
            if (combinedNotes.Contains("allergy") || combinedNotes.Contains("allergic"))
            {
                if (combinedNotes.Contains(medicineName.ToLower()))
                {
                    result.IsSafe = false;
                    result.WarningLevel = "Critical";
                    result.Conflicts.Add($"Suspected allergy to {medicineName} mentioned in history.");
                }
            }

            if (!result.IsSafe)
            {
                result.Message = $"SAFETY ALERT: {string.Join(" ", result.Conflicts)}";
            }
            else
            {
                result.Message = "No high-risk contraindications detected.";
            }

            return result;
        }

        public async Task<SafetyCheckResult> AnalyzeTestResultAsync(LabViewModel lab)
        {
            var result = new SafetyCheckResult { IsSafe = true, WarningLevel = "None" };
            var AIInsights = new System.Text.StringBuilder();
            AIInsights.AppendLine("### AI Diagnostic Summary");
            
            // 1. Analyze BMI
            double bmi = lab.Height > 0 ? (lab.Weight / System.Math.Pow(lab.Height / 100.0, 2)) : 0;
            string bmiStatus = "Normal";
            if (bmi < 18.5) bmiStatus = "Low (Underweight)";
            else if (bmi > 25 && bmi < 30) bmiStatus = "High (Overweight)";
            else if (bmi >= 30) bmiStatus = "Very High (Obese)";
            
            AIInsights.AppendLine($"- **BMI Analysis**: {bmi:F1} ({bmiStatus})");

            // 2. Analyze BP
            if (lab.BloodPressure > 0)
            {
                string bpStatus = "Normal";
                if (lab.BloodPressure >= 140) { bpStatus = "High (Hypertension)"; result.IsSafe = false; result.WarningLevel = "Moderate"; }
                else if (lab.BloodPressure < 90) bpStatus = "Low";
                AIInsights.AppendLine($"- **Blood Pressure**: {lab.BloodPressure} mmHg ({bpStatus})");
            }

            // 3. Analyze Temperature
            if (lab.Temperature >= 38)
            {
                AIInsights.AppendLine($"- **Temperature**: {lab.Temperature}°C (High - Fever detected)");
                result.IsSafe = false;
                result.WarningLevel = "Moderate";
            }
            else if (lab.Temperature < 36)
            {
                AIInsights.AppendLine($"- **Temperature**: {lab.Temperature}°C (Low - Hypothermia risk)");
            }
            else if (lab.Temperature > 0)
            {
                AIInsights.AppendLine($"- **Temperature**: {lab.Temperature}°C (Normal)");
            }

            // 4. Test Value Reference Range Analysis
            if (lab.TestValue.HasValue && !string.IsNullOrWhiteSpace(lab.TestCode))
            {
                string code = lab.TestCode.Trim().ToUpper();
                double val = lab.TestValue.Value;
                string status = "Normal";

                if (code == "CBC" || code == "HEMOGLOBIN")
                {
                    if (val < 12.0) status = "Low (Anemia risk)";
                    else if (val > 17.5) status = "High (Polycythemia risk)";
                }
                else if (code == "TSH")
                {
                    if (val < 0.4) status = "Low (Hyperthyroidism risk)";
                    else if (val > 4.0) status = "High (Hypothyroidism risk)";
                }
                else if (code == "GLUCOSE" || code == "FBS" || code == "SUGAR")
                {
                    if (val < 70) status = "Low (Hypoglycemia)";
                    else if (val > 100) status = "High (Prediabetes/Diabetes risk)";
                }
                else
                {
                    status = "Value Recorded (No standard AI reference range applied)";
                }

                if (status.Contains("High") || status.Contains("Low"))
                {
                    result.IsSafe = false;
                    result.WarningLevel = "Moderate";
                }

                AIInsights.AppendLine($"- **{code} Test Value**: {val} - **Status: {status}**");
            }

            // 5. Test Result Pattern Matching
            if (!string.IsNullOrEmpty(lab.TestResults) && lab.TestResults.ToLower().Contains("high"))
            {
                AIInsights.AppendLine("- **Note**: Additional manual notes mention 'High' values. Detailed review recommended.");
            }

            AIInsights.AppendLine();
            AIInsights.AppendLine("**Recommendation**: Continue monitoring vitals. " + (result.IsSafe ? "Patient is currently stable." : "Please review the flagged out-of-range diagnostics above."));

            result.Message = AIInsights.ToString();
            return result;
        }
    }
}
