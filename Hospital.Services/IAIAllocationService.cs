using Hospital.ViewModels;
using System.Threading.Tasks;
using Hospital.Models;

namespace Hospital.Services
{
    public interface IAIAllocationService
    {
        Task<RoomAllocationViewModel> SuggestOptimalRoom(int hospitalId, RoomType preferredType);
    }
}
