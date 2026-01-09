using MedNet.API.Models;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class LabAnalysisService : ILabAnalysisService
    {
        private readonly ILabAnalysisRepository labAnalysisRepository;
        private readonly ILabTestService labTestService;
        private readonly ILogger<LabAnalysisService> logger;
        public LabAnalysisService(ILabAnalysisRepository labAnalysisRepository, ILabTestService labTestService, ILogger<LabAnalysisService> logger)
        {
            this.labAnalysisRepository = labAnalysisRepository;
            this.labTestService = labTestService;
            this.logger = logger;
        }
        public async Task<LabAnalysisDto> CreateLabAnalysisAsync(CreateLabAnalysisRequestDto request)
        {
            logger.LogInformation("Creating lab analysis for Patient {PatientId}, Type: {AnalysisType}, Tests: {TestCount}",
                request.PatientId, request.AnalysisType, request.LabTests?.Count ?? 0);

            var labAnalysis = new LabAnalysis
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                AnalysisDate = request.AnalysisDate,
                AnalysisType = request.AnalysisType
            };

            var createdLabAnalysis = await labAnalysisRepository.CreateAsync(labAnalysis);

            logger.LogDebug("Lab analysis {AnalysisId} created, now creating {TestCount} lab tests",
                createdLabAnalysis.Id, request.LabTests?.Count ?? 0);

            foreach (var testDto in request.LabTests)
            {
                var createTestRequest = new CreateLabTestRequestDto
                {
                    LabAnalysisId = createdLabAnalysis.Id,
                    TestName = testDto.TestName,
                    Result = testDto.Result,
                    Units = testDto.Units,
                    ReferenceRange = testDto.ReferenceRange
                };

                await labTestService.CreateLabTestAsync(createTestRequest);
            }

            var labAnalysisWithTests = await labAnalysisRepository.GetById(createdLabAnalysis.Id);

            logger.LogInformation("Lab analysis {AnalysisId} created successfully for Patient {PatientId} with {TestCount} tests - Type: {AnalysisType}",
                labAnalysisWithTests.Id, labAnalysisWithTests.PatientId, labAnalysisWithTests.LabTests.Count, labAnalysisWithTests.AnalysisType);

            return new LabAnalysisDto
            {
                Id = labAnalysisWithTests.Id,
                PatientId = labAnalysisWithTests.PatientId,
                AnalysisDate = labAnalysisWithTests.AnalysisDate,
                AnalysisType = labAnalysisWithTests.AnalysisType,
                LabTests = labAnalysisWithTests.LabTests.Select(t => new LabTestDto
                {
                    Id = t.Id,
                    TestName = t.TestName,
                    Result = t.Result,
                    Units = t.Units,
                    ReferenceRange = t.ReferenceRange
                }).ToList()
            };
        }

        public async Task<IEnumerable<DisplayLabAnalysisDto>> GetAllLabAnalysesAsync()
        {
            logger.LogInformation("Retrieving all lab analyses");

            var labAnalyses = await labAnalysisRepository.GetAllAsync();

            var labTestDtos = await labTestService.GetAllLabTestsAsync();

            var analysisList = labAnalyses.Select(labAnalysis => new DisplayLabAnalysisDto
            {
                Id = labAnalysis.Id,
                PatientId = labAnalysis.PatientId,
                AnalysisDate = labAnalysis.AnalysisDate,
                AnalysisType = labAnalysis.AnalysisType,
                LabTests = labTestDtos
                    .Where(dto => labAnalysis.LabTests.Select(lt => lt.Id).Contains(dto.Id))
                    .Select(dto => new DisplayLabTestDto
                    {
                        TestName = dto.TestName,
                        Result = dto.Result,
                        Units = dto.Units,
                        ReferenceRange = dto.ReferenceRange
                    }).ToList()
            }).ToList();

            logger.LogInformation("Retrieved {Count} lab analyses", analysisList.Count);

            return analysisList;
        }

        public async Task<DisplayLabAnalysisDto?> GetLabAnalysisByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving lab analysis with ID: {AnalysisId}", id);

            var labAnalysis = await labAnalysisRepository.GetById(id);
            if (labAnalysis == null)
            {
                logger.LogWarning("Lab analysis not found with ID: {AnalysisId}", id);
                return null;
            }

            var labTestDtos = await labTestService.GetAllLabTestsAsync();

            logger.LogInformation("Lab analysis {AnalysisId} retrieved - Patient: {PatientId}, Type: {AnalysisType}, Tests: {TestCount}",
                labAnalysis.Id, labAnalysis.PatientId, labAnalysis.AnalysisType, labAnalysis.LabTests.Count);

            return new DisplayLabAnalysisDto
            {
                Id = labAnalysis.Id,
                PatientId = labAnalysis.PatientId,
                AnalysisDate = labAnalysis.AnalysisDate,
                AnalysisType = labAnalysis.AnalysisType,
                LabTests = labTestDtos
                    .Where(dto => labAnalysis.LabTests.Select(lt => lt.Id).Contains(dto.Id))
                    .Select(dto => new DisplayLabTestDto
                    {
                        TestName = dto.TestName,
                        Result = dto.Result,
                        Units = dto.Units,
                        ReferenceRange = dto.ReferenceRange
                    }).ToList()
            };
        }
        public async Task<UpdatedLabAnalysisDto?> UpdateLabAnalysisAsync(Guid id, UpdateLabAnalysisRequestDto request)
        {
            logger.LogInformation("Updating lab analysis with ID: {AnalysisId}", id);

            var existingLabAnalysis = await labAnalysisRepository.GetById(id);

            if (existingLabAnalysis == null)
            {
                logger.LogWarning("Lab analysis not found for update with ID: {AnalysisId}", id);
                return null;
            }

            var oldAnalysisType = existingLabAnalysis.AnalysisType;
            var oldAnalysisDate = existingLabAnalysis.AnalysisDate;

            existingLabAnalysis.AnalysisDate = request.AnalysisDate;
            existingLabAnalysis.AnalysisType = request.AnalysisType;

            var updatedLabAnalysis = await labAnalysisRepository.UpdateAsync(existingLabAnalysis);

            if (updatedLabAnalysis == null)
            {
                logger.LogError("Failed to update lab analysis with ID: {AnalysisId}", id);
                return null;
            }

            logger.LogInformation("Lab analysis {AnalysisId} updated successfully - Type: '{OldType}' → '{NewType}', Date: {OldDate} → {NewDate}",
                id, oldAnalysisType, updatedLabAnalysis.AnalysisType, oldAnalysisDate.ToShortDateString(), updatedLabAnalysis.AnalysisDate.ToShortDateString());

            return new UpdatedLabAnalysisDto
            {
                Id = updatedLabAnalysis.Id,
                AnalysisDate = updatedLabAnalysis.AnalysisDate,
                AnalysisType = updatedLabAnalysis.AnalysisType
            };
        }
        public async Task<string?> DeleteLabAnalysisAsync(Guid id)
        {
            logger.LogInformation("Deleting lab analysis with ID: {AnalysisId}", id);

            var labAnalysis = await labAnalysisRepository.DeleteAsync(id);

            if (labAnalysis == null)
            {
                logger.LogWarning("Lab analysis not found for deletion with ID: {AnalysisId}", id);
                return null;
            }

            logger.LogInformation("Lab analysis {AnalysisId} deleted successfully - Patient: {PatientId}, Type: {AnalysisType}",
                labAnalysis.Id, labAnalysis.PatientId, labAnalysis.AnalysisType);

            return $"Lab analysis '{labAnalysis.AnalysisType}' (ID: {labAnalysis.Id}) deleted successfully!";
        }
    }
}
