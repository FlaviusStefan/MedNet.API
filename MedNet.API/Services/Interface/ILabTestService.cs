using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface ILabTestService
    {
        
        Task<LabTestDto> CreateLabTestAsync(CreateLabTestRequestDto requestDto, Guid labAnalysisId);
        Task<IEnumerable<LabTestDto>> GetAllLabTestsAsync();
        Task<LabTestDto> GetLabTestByIdAsync(Guid id);
        Task<LabTestDto> UpdateLabTestAsync(Guid id, UpdateLabTestRequestDto request);
        Task<LabTestDto> DeleteLabTestAsync(Guid id);
    }
}
