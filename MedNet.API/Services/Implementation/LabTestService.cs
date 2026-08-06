using AutoMapper;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository labTestRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<LabTestService> logger;
        private readonly IMapper mapper;


        public LabTestService(ILabTestRepository labTestRepository, IUnitOfWork unitOfWork, ILogger<LabTestService> logger, IMapper mapper)
        {
            this.labTestRepository = labTestRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<LabTestDto> CreateLabTestAsync(CreateLabTestRequestDto request)
        {
            logger.LogInformation("Creating lab test for LabAnalysis {AnalysisId}, Test: {TestName}, Result: {Result}",
                request.LabAnalysisId, request.TestName, request.Result);

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

            logger.LogInformation("Lab test {TestId} created successfully for LabAnalysis {AnalysisId} - {TestName}: {Result} {Units}",
                createdLabTest.Id, createdLabTest.LabAnalysisId, createdLabTest.TestName, createdLabTest.Result, createdLabTest.Units);

            return mapper.Map<LabTestDto>(createdLabTest);
        }

        public async Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync()
        {
            logger.LogInformation("Retrieving all lab tests");

            var labTests = await labTestRepository.GetAllAsync();
            var testList = mapper.Map<List<LabTestDto>>(labTests);

            logger.LogInformation("Retrieved {Count} lab tests", testList.Count);

            return testList;
        }

        public async Task<LabTestDto> GetLabTestByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving lab test with ID: {TestId}", id);

            var labTest = await labTestRepository.GetById(id);
            if (labTest is null)
            {
                logger.LogWarning("Lab test not found with ID: {TestId}", id);
                return null;
            }

            logger.LogInformation("Lab test {TestId} retrieved - LabAnalysis: {AnalysisId}, Test: {TestName}, Result: {Result}",
                labTest.Id, labTest.LabAnalysisId, labTest.TestName, labTest.Result);

            return mapper.Map<LabTestDto>(labTest);
        }

        public async Task<LabTestDto> UpdateLabTestAsync(Guid id, UpdateLabTestRequestDto request)
        {
            logger.LogInformation("Updating lab test with ID: {TestId}", id);

            var existingLabTest = await labTestRepository.GetById(id);
            if (existingLabTest is null)
            {
                logger.LogWarning("Lab test not found for update with ID: {TestId}", id);
                return null;
            }

            var oldTestName = existingLabTest.TestName;
            var oldResult = existingLabTest.Result;

            var labTestToUpdate = new LabTest
            {
                Id = id,
                LabAnalysisId = existingLabTest.LabAnalysisId,
                TestName = request.TestName,
                Result = request.Result,
                Units = request.Units,
                ReferenceRange = request.ReferenceRange
            };

            var updatedLabTest = await labTestRepository.UpdateAsync(labTestToUpdate);

            if (updatedLabTest is null)
            {
                logger.LogError("Failed to update lab test with ID: {TestId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Lab test {TestId} updated successfully - Test: '{OldName}' → '{NewName}', Result: '{OldResult}' → '{NewResult}'",
                id, oldTestName, updatedLabTest.TestName, oldResult, updatedLabTest.Result);

            return mapper.Map<LabTestDto>(updatedLabTest);
        }

        public async Task<string?> DeleteLabTestAsync(Guid id)
        {
            logger.LogInformation("Deleting lab test with ID: {TestId}", id);

            var labTest = await labTestRepository.DeleteAsync(id);

            if (labTest is null)
            {
                logger.LogWarning("Lab test not found for deletion with ID: {TestId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Lab test {TestId} deleted successfully - LabAnalysis: {AnalysisId}, Test: {TestName}",
                labTest.Id, labTest.LabAnalysisId, labTest.TestName);

            return $"Lab test '{labTest.TestName}' (ID: {labTest.Id}) deleted successfully!";
        }
    }
}