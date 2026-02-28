using System;
using System.Collections.Generic;
using Hospital.Models;

namespace Hospital.ViewModels
{
    public class LabViewModel
    {
        public int Id { get; set; }
        public string LabNumber { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string TestType { get; set; }
        public string TestCode { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }
        public int BloodPressure { get; set; }
        public int Temperature { get; set; }
        public double? TestValue { get; set; }
        public string TestResults { get; set; }
        public DateTime CreatedAt { get; set; }

        public LabViewModel() { }

        public LabViewModel(Lab model)
        {
            Id = model.Id;
            LabNumber = model.LabNumber;
            TestType = model.TestType;
            TestCode = model.TestCode;
            Weight = model.Weight;
            Height = model.Height;
            BloodPressure = model.BloodPressure;
            Temperature = model.Temperature;
            TestValue = model.TestValue;
            TestResults = model.TestResults;
            CreatedAt = model.CreatedAt;
            if (model.Patient != null)
            {
                PatientId = model.Patient.Id;
                PatientName = model.Patient.Name;
            }
        }

        public Lab ConvertViewModel(LabViewModel vm)
        {
            return new Lab
            {
                Id = vm.Id,
                LabNumber = vm.LabNumber,
                PatientId = vm.PatientId,
                TestType = vm.TestType,
                TestCode = vm.TestCode,
                Weight = vm.Weight,
                Height = vm.Height,
                BloodPressure = vm.BloodPressure,
                Temperature = vm.Temperature,
                TestValue = vm.TestValue,
                TestResults = vm.TestResults,
                CreatedAt = vm.CreatedAt
            };
        }
    }
}
