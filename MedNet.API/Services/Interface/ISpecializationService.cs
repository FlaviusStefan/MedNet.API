using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface ISpecializationService
    {
        Task<SpecializationDto> CreateSpecializationAsync(CreateSpecializationRequestDto request);
        Task<IEnumerable<SpecializationDto>> GetAllSpecializationsAsync();
        Task<SpecializationDto?> GetSpecializationByIdAsync(Guid id);
        Task<SpecializationDto?> UpdateSpecializationAsync(Guid id, UpdateSpecializationRequestDto request);
        Task<SpecializationDto?> DeleteSpecializationAsync(Guid id);
    }
}
