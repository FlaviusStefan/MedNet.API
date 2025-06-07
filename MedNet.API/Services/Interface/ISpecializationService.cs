using MedNet.API.Models.DTO;
using Microsoft.EntityFrameworkCore.Storage;

namespace MedNet.API.Services.Interface
{
    public interface ISpecializationService
    {
        Task<SpecializationDto> CreateSpecializationAsync(CreateSpecializationRequestDto request);
        Task<IEnumerable<SpecializationDto>> GetAllSpecializationsAsync();
        Task<SpecializationDto?> GetSpecializationByIdAsync(Guid id);
        Task<IEnumerable<SpecializationDto>> GetSpecializationsByDoctorIdAsync(Guid doctorId);
        Task<SpecializationDto?> UpdateSpecializationAsync(Guid id, UpdateSpecializationRequestDto request);
        Task<SpecializationDto?> DeleteSpecializationAsync(Guid id);
        Task<Dictionary<Guid, string>> ValidateSpecializationsAsync(IEnumerable<Guid> specializationIds);


    }
}
