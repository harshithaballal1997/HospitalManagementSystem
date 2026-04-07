using System.Security.Claims;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IAIChatService
    {
        Task<string> ProcessQueryAsync(string query, ClaimsPrincipal user);
    }
}
