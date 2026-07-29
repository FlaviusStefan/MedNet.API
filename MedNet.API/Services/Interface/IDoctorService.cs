using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IDoctorService
    {
        Task<CreatedDoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request);
        Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync();
        Task<DoctorResponseDto?> GetDoctorByIdAsync(Guid id);
        Task<UpdatedDoctorDto?> UpdateDoctorAsync(Guid id, UpdateDoctorRequestDto request);
        Task<string?> DeleteDoctorAsync(Guid id);
    }
}
