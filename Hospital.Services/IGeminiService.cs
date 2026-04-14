using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateResponseAsync(string prompt);
    }
}
