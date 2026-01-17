using Hospital.Utilities;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Services
{
    public interface IDoctorService
    {
        PagedResult<TimmingViewModel> GetAll(int pageNumber, int pageSize);
        IEnumerable<TimmingViewModel> GetAll();
        TimmingViewModel GetTimmingById(int TimmingId);
        void UpdateTiming(TimmingViewModel timming);
        void AddTiming(TimmingViewModel timming);
        void DeleteTiming(int TimmingId);
    }
}
