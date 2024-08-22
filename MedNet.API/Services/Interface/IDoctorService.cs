using MedNet.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Services
{
    public interface IDoctorService
    {
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(Guid id);
        Task<DoctorDto?> UpdateDoctorAsync(Guid id, UpdateDoctorRequestDto request);
        Task<string?> DeleteDoctorAsync(Guid id);
    }
}
