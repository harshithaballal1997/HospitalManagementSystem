using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IMedicalAssistantService
    {
        Task<string> SummarizePatientHistoryAsync(string patientId);
    }
}
