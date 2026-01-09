using MedNet.API.Models;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository labTestRepository;
        private readonly ILogger<LabTestService> logger;

        public LabTestService(ILabTestRepository labTestRepository, ILogger<LabTestService> logger)
        {
            this.labTestRepository = labTestRepository;
            this.logger = logger;
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
            logger.LogInformation("Retrieving all lab tests");

            var labTests = await labTestRepository.GetAllAsync();

            var testList = labTests.Select(labTest => new LabTestDto
            {
                Id = labTest.Id,
                LabAnalysisId = labTest.LabAnalysisId,
                TestName = labTest.TestName,
                Result = labTest.Result,
                Units = labTest.Units,
                ReferenceRange = labTest.ReferenceRange
            }).ToList();

            logger.LogInformation("Retrieved {Count} lab tests", testList.Count);

            return testList;
        }

        public async Task<LabTestDto> GetLabTestByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving lab test with ID: {TestId}", id);

            var labTest = await labTestRepository.GetById(id);
            if (labTest == null)
            {
                logger.LogWarning("Lab test not found with ID: {TestId}", id);
                return null;
            }

            logger.LogInformation("Lab test {TestId} retrieved - LabAnalysis: {AnalysisId}, Test: {TestName}, Result: {Result}",
                labTest.Id, labTest.LabAnalysisId, labTest.TestName, labTest.Result);

            return new LabTestDto
            {
                Id = labTest.Id,
                LabAnalysisId = labTest.LabAnalysisId,
                TestName = labTest.TestName,
                Result = labTest.Result,
                Units = labTest.Units,
                ReferenceRange = labTest.ReferenceRange
            };
        }

        public async Task<LabTestDto> UpdateLabTestAsync(Guid id, UpdateLabTestRequestDto request)
        {
            logger.LogInformation("Updating lab test with ID: {TestId}", id);

            var existingLabTest = await labTestRepository.GetById(id);
            if (existingLabTest == null)
            {
                logger.LogWarning("Lab test not found for update with ID: {TestId}", id);
                return null;
            }

            var oldTestName = existingLabTest.TestName;
            var oldResult = existingLabTest.Result;

            existingLabTest.TestName = request.TestName;
            existingLabTest.Result = request.Result;
            existingLabTest.Units = request.Units;
            existingLabTest.ReferenceRange = request.ReferenceRange;

            var updatedLabTest = await labTestRepository.UpdateAsync(existingLabTest);

            if (updatedLabTest == null)
            {
                logger.LogError("Failed to update lab test with ID: {TestId}", id);
                return null;
            }

            logger.LogInformation("Lab test {TestId} updated successfully - Test: '{OldName}' → '{NewName}', Result: '{OldResult}' → '{NewResult}'",
                id, oldTestName, updatedLabTest.TestName, oldResult, updatedLabTest.Result);

            return new LabTestDto
            {
                Id = updatedLabTest.Id,
                TestName = updatedLabTest.TestName,
                Result = updatedLabTest.Result,
                Units = updatedLabTest.Units,
                ReferenceRange = updatedLabTest.ReferenceRange
            };
        }

        public async Task<string?> DeleteLabTestAsync(Guid id)
        {
            logger.LogInformation("Deleting lab test with ID: {TestId}", id);

            var labTest = await labTestRepository.DeleteAsync(id);

            if (labTest == null)
            {
                logger.LogWarning("Lab test not found for deletion with ID: {TestId}", id);
                return null;
            }

            logger.LogInformation("Lab test {TestId} deleted successfully - LabAnalysis: {AnalysisId}, Test: {TestName}",
                labTest.Id, labTest.LabAnalysisId, labTest.TestName);

            return $"Lab test '{labTest.TestName}' (ID: {labTest.Id}) deleted successfully!";
        }
    }
}