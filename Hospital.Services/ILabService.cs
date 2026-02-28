using Hospital.ViewModels;
using Hospital.Utilities;
using System.Collections.Generic;

namespace Hospital.Services
{
    public interface ILabService
    {
        PagedResult<LabViewModel> GetAll(int pageNumber, int pageSize);
        IEnumerable<LabViewModel> GetAll();
        LabViewModel GetLabById(int labId);
        void UpdateLab(LabViewModel lab);
        void AddLab(LabViewModel lab);
        void DeleteLab(int labId);
        IEnumerable<LabViewModel> GetLabsByPatientId(string patientId);
        PagedResult<LabViewModel> GetLatestLabsPaged(int pageNumber, int pageSize);
    }
}
