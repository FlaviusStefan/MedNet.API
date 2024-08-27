using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IHospitalService
    {
        Task<HospitalDto> CreateHospitalAsync(CreateHospitalRequestDto request);
        Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync();
        Task<HospitalDto?> GetHospitalByIdAsync(Guid id);
        Task<HospitalDto?> UpdateHospitalAsync(Guid id, UpdateHospitalRequestDto request);
        Task<string?> DeleteHospitalAsync(Guid id);
    }
}
