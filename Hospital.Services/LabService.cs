using Hospital.Models;
using Hospital.Repositories.Interfaces;
using Hospital.Utilities;
using Hospital.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hospital.Services
{
    public class LabService : ILabService
    {
        private IUnitOfWork _unitOfWork;

        public LabService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddLab(LabViewModel lab)
        {
            var model = lab.ConvertViewModel(lab);
            _unitOfWork.GenericRepository<Lab>().Add(model);
            _unitOfWork.Save();
        }

        public void DeleteLab(int labId)
        {
            var model = _unitOfWork.GenericRepository<Lab>().GetById(labId);
            _unitOfWork.GenericRepository<Lab>().Delete(model);
            _unitOfWork.Save();
        }

        public PagedResult<LabViewModel> GetAll(int pageNumber, int pageSize)
        {
            int totalCount;
            List<LabViewModel> vmList = new List<LabViewModel>();
            try
            {
                int excludeRecords = (pageSize * pageNumber) - pageSize;
                var modelList = _unitOfWork.GenericRepository<Lab>().GetAll(includeProperties: "Patient").Skip(excludeRecords).Take(pageSize).ToList();
                totalCount = _unitOfWork.GenericRepository<Lab>().GetAll().Count();
                vmList = ConvertModelToViewModelList(modelList);
            }
            catch (Exception)
            {
                throw;
            }
            var result = new PagedResult<LabViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        public PagedResult<LabViewModel> GetLatestLabsPaged(int pageNumber, int pageSize)
        {
            int totalCount;
            List<LabViewModel> vmList = new List<LabViewModel>();
            try
            {
                // Group by patient and get only the most recent lab for each patient
                var allLabs = _unitOfWork.GenericRepository<Lab>().GetAll(includeProperties: "Patient").ToList();
                
                var latestLabs = allLabs
                    .GroupBy(l => l.PatientId)
                    .Select(g => g.OrderByDescending(l => l.CreatedAt).FirstOrDefault())
                    .Where(l => l != null)
                    .OrderByDescending(l => l.CreatedAt)
                    .ToList();

                totalCount = latestLabs.Count;
                int excludeRecords = (pageSize * pageNumber) - pageSize;
                var pagedModelList = latestLabs.Skip(excludeRecords).Take(pageSize).ToList();
                
                vmList = ConvertModelToViewModelList(pagedModelList);
            }
            catch (Exception)

            {
                throw;
            }
            var result = new PagedResult<LabViewModel>
            {
                Data = vmList,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return result;
        }

        public IEnumerable<LabViewModel> GetAll()
        {
            var labList = _unitOfWork.GenericRepository<Lab>().GetAll(includeProperties: "Patient").ToList();
            return ConvertModelToViewModelList(labList);
        }

        public LabViewModel GetLabById(int labId)
        {
            var model = _unitOfWork.GenericRepository<Lab>().GetAll(filter: l => l.Id == labId, includeProperties: "Patient").FirstOrDefault();
            return new LabViewModel(model);
        }

        public void UpdateLab(LabViewModel lab)
        {
            var modelById = _unitOfWork.GenericRepository<Lab>().GetById(lab.Id);
            modelById.LabNumber = lab.LabNumber;
            modelById.PatientId = lab.PatientId;
            modelById.TestType = lab.TestType;
            modelById.TestCode = lab.TestCode;
            modelById.Weight = lab.Weight;
            modelById.Height = lab.Height;
            modelById.BloodPressure = lab.BloodPressure;
            modelById.Temperature = lab.Temperature;
            modelById.TestResults = lab.TestResults;
            
            _unitOfWork.GenericRepository<Lab>().Update(modelById);
            _unitOfWork.Save();
        }

        public IEnumerable<LabViewModel> GetLabsByPatientId(string patientId)
        {
            var labList = _unitOfWork.GenericRepository<Lab>().GetAll(filter: l => l.PatientId == patientId, includeProperties: "Patient").OrderByDescending(l => l.Id).ToList();
            return ConvertModelToViewModelList(labList);
        }

        private List<LabViewModel> ConvertModelToViewModelList(List<Lab> modelList)
        {
            return modelList.Select(x => new LabViewModel(x)).ToList();
        }
    }
}
