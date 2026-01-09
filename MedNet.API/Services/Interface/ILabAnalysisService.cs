using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface ILabAnalysisService
    {
        Task<LabAnalysisDto> CreateLabAnalysisAsync(CreateLabAnalysisRequestDto request);
        Task<IEnumerable<DisplayLabAnalysisDto>> GetAllLabAnalysesAsync();
        Task<DisplayLabAnalysisDto?> GetLabAnalysisByIdAsync(Guid id);
        Task<UpdatedLabAnalysisDto> UpdateLabAnalysisAsync(Guid id, UpdateLabAnalysisRequestDto request);
        Task<string?> DeleteLabAnalysisAsync(Guid id);
    }
}
