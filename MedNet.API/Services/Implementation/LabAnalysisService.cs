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

        public LabAnalysisService(ILabAnalysisRepository labAnalysisRepository, ILabTestService labTestService)
        {
            this.labAnalysisRepository = labAnalysisRepository;
            this.labTestService = labTestService;
        }

        public async Task<LabAnalysisDto> CreateLabAnalysisAsync(CreateLabAnalysisRequestDto request)
        {
            var labAnalysis = new LabAnalysis
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                AnalysisDate = request.AnalysisDate,
                AnalysisType = request.AnalysisType
            };

            var createdLabAnalysis = await labAnalysisRepository.CreateAsync(labAnalysis);

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



        public async Task<IEnumerable<LabAnalysisDto>> GetAllLabAnalysesAsync()
        {
            var labAnalyses = await labAnalysisRepository.GetAllAsync();

            var labTestDtos = await labTestService.GetAllLabTestsAsync();

            return labAnalyses.Select(labAnalysis => new LabAnalysisDto
            {
                Id = labAnalysis.Id,
                PatientId = labAnalysis.PatientId,
                AnalysisDate = labAnalysis.AnalysisDate,
                AnalysisType = labAnalysis.AnalysisType,
                LabTests = labTestDtos
                    .Where(dto => labAnalysis.LabTests.Select(lt => lt.Id).Contains(dto.Id))
                    .Select(dto => new LabTestDto
                    {
                        Id = dto.Id,
                        TestName = dto.TestName,
                        Result = dto.Result,
                        Units = dto.Units,
                        ReferenceRange = dto.ReferenceRange
                    }).ToList()
            });
            
        }

        public async Task<LabAnalysisDto?> GetLabAnalysisByIdAsync(Guid id)
        {
            var labAnalysis = await labAnalysisRepository.GetById(id);
            if(labAnalysis == null)
            {
                return null;
            }

            var labTestDtos = await labTestService.GetAllLabTestsAsync();


            return new LabAnalysisDto
            {
                Id = labAnalysis.Id,
                PatientId = labAnalysis.PatientId,
                AnalysisDate = labAnalysis.AnalysisDate,
                AnalysisType = labAnalysis.AnalysisType,
                LabTests = labTestDtos
                    .Where(dto => labAnalysis.LabTests.Select(lt => lt.Id).Contains(dto.Id))
                    .Select(dto => new LabTestDto
                    {
                        Id = dto.Id,
                        TestName = dto.TestName,
                        Result = dto.Result,
                        Units = dto.Units,
                        ReferenceRange = dto.ReferenceRange
                    }).ToList()
            };
        }

        public async Task<UpdatedLabAnalysisDto?> UpdateLabAnalysisAsync(Guid id, UpdateLabAnalysisRequestDto request)
        {
            var existingLabAnalysis = await labAnalysisRepository.GetById(id);

            if (existingLabAnalysis == null) return null;

            existingLabAnalysis.AnalysisDate = request.AnalysisDate;
            existingLabAnalysis.AnalysisType = request.AnalysisType;

            var updatedLabAnalysis = await labAnalysisRepository.UpdateAsync(existingLabAnalysis);

            if (updatedLabAnalysis == null) return null;

            return new UpdatedLabAnalysisDto
            {
                Id = updatedLabAnalysis.Id,
                AnalysisDate = updatedLabAnalysis.AnalysisDate,
                AnalysisType = updatedLabAnalysis.AnalysisType
            };
        }


        public async Task<LabAnalysisDto> DeleteLabAnalysisAsync(Guid id)
        {
            var labAnalysis = await labAnalysisRepository.DeleteAsync(id);

            if (labAnalysis == null) return null;

            return new LabAnalysisDto
            {
                Id = labAnalysis.Id,
                AnalysisDate = labAnalysis.AnalysisDate,
                AnalysisType = labAnalysis.AnalysisType
            };
        }
    }
}
