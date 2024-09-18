using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface ILabAnalysisService
    {
        Task<LabAnalysisDto> CreateLabAnalysisAsync(CreateLabAnalysisRequestDto request);
        Task<IEnumerable<LabAnalysisDto>> GetAllLabAnalysesAsync();
        Task<LabAnalysisDto?> GetLabAnalysisByIdAsync(Guid id);
        Task<LabAnalysisDto> UpdateLabAnalysisAsync(Guid id, UpdateLabAnalysisRequestDto request);
        Task<LabAnalysisDto> DeleteLabAnalysisAsync(Guid id);
    }
}
