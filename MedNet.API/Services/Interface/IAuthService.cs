using MedNet.API.Models.DTO.Auth;

namespace MedNet.API.Services.Interface
{
    public interface IAuthService
    {
        Task<string> RegisterPatientAsync(RegisterPatientDto registerDto);
        Task<string> RegisterPatientByAdminAsync(RegisterPatientByAdminDto registerPatientByAdminDto);
        Task<string> RegisterDoctorAsync(RegisterDoctorDto registerDto);
        Task<string> RegisterDoctorByAdminAsync(RegisterDoctorByAdminDto registerDoctorByAdminDto);
        Task<string> RegisterHospitalByAdminAsync(RegisterHospitalByAdminDto registerHospitalByAdminDto);
        Task<string> LoginAsync(LoginDto loginDto);
    }
}
