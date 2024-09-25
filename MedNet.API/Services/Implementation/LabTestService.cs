using MedNet.API.Models;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository labTestRepository;

        public LabTestService(ILabTestRepository labTestRepository)
        {
            this.labTestRepository = labTestRepository;
        }

        public async Task<LabTestDto> CreateLabTestAsync(CreateLabTestRequestDto request)
        {
            var labTest = new LabTest
            {
                Id = Guid.NewGuid(),
                LabAnalysisId = request.LabAnalysisId,
                TestName = request.TestName,
                Result = request.Result,
                Units = request.Units,
                ReferenceRange = request.ReferenceRange
            };

            var createdLabTest = await labTestRepository.CreateAsync(labTest);

            return new LabTestDto
            {
                Id = createdLabTest.Id,
                LabAnalysisId = createdLabTest.LabAnalysisId,
                TestName = createdLabTest.TestName,
                Result = createdLabTest.Result,
                Units = createdLabTest.Units,
                ReferenceRange = createdLabTest.ReferenceRange
            };
        }

        public async Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync()
        {
            var labTests = await labTestRepository.GetAllAsync();

            return labTests.Select(labTest => new LabTestDto
            {
                Id = labTest.Id,
                TestName = labTest.TestName,
                Result = labTest.Result,
                Units = labTest.Units,
                ReferenceRange = labTest.ReferenceRange
            }).ToList();
        }

        public async Task<LabTestDto> GetLabTestByIdAsync(Guid id)
        {
            var labTest = await labTestRepository.GetById(id);
            if (labTest == null)
            {
                return null;
            }

            return new LabTestDto
            {
                Id = labTest.Id,
                TestName = labTest.TestName,
                Result = labTest.Result,
                Units = labTest.Units,
                ReferenceRange = labTest.ReferenceRange
            };
        }

        public async Task<LabTestDto> UpdateLabTestAsync(Guid id, UpdateLabTestRequestDto request)
        {
            var existingLabTest = await labTestRepository.GetById(id);
            if (existingLabTest == null) return null;

            existingLabTest.TestName = request.TestName;
            existingLabTest.Result = request.Result;
            existingLabTest.Units = request.Units;
            existingLabTest.ReferenceRange = request.ReferenceRange;

            var updatedLabTest = await labTestRepository.UpdateAsync(existingLabTest);

            if (updatedLabTest == null) return null;

            return new LabTestDto
            {
                Id = updatedLabTest.Id,
                TestName = updatedLabTest.TestName,
                Result = updatedLabTest.Result,
                Units = updatedLabTest.Units,
                ReferenceRange = updatedLabTest.ReferenceRange
            };
        }


        public async Task<LabTestDto> DeleteLabTestAsync(Guid id)
        {
            var labTest = await labTestRepository.DeleteAsync(id);

            if (labTest == null) return null;

            return new LabTestDto
            {
                Id = labTest.Id,
                TestName = labTest.TestName,
                Result = labTest.Result,
                Units = labTest.Units,
                ReferenceRange = labTest.ReferenceRange
            };
        }

    }
}