using MedNet.API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MedNet.API.Services
{
    public interface IDoctorService
    {
        Task<CreatedDoctorDto> CreateDoctorAsync(CreateDoctorRequestDto request);
        Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync();
        Task<DoctorDto?> GetDoctorByIdAsync(Guid id);
        Task<UpdatedDoctorDto?> UpdateDoctorAsync(Guid id, UpdateDoctorRequestDto request);
        Task<string?> DeleteDoctorAsync(Guid id);
    }
}
