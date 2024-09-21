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

        public async Task<LabTestDto> CreateLabTestAsync(CreateLabTestRequestDto request,Guid labAnalysisId)
        {
            var labTest = new LabTest
            {
                Id = Guid.NewGuid(),
                LabAnalysisId = labAnalysisId,
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

        public Task<LabTestDto> GetLabTestByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<LabTestDto> UpdateLabTestAsync(Guid id, UpdateLabTestRequestDto request)
        {
            throw new NotImplementedException();
        }
        public Task<LabTestDto> DeleteLabTestAsync(Guid id)
        {
            throw new NotImplementedException();
        }

    }
}
