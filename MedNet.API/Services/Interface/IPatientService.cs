using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IPatientService
    {
        Task<CreatedPatientDto> CreatePatientAsync(CreatePatientRequestDto request);
        Task<IEnumerable<PatientBasicSummaryDto>> GetAllPatientsAsync();
        Task<PatientDto?> GetPatientByIdAsync(Guid id);
        Task<PatientDto?> GetPatientByUserIdAsync(string userId); 
        Task<UpdatedPatientDto?> UpdatePatientAsync(Guid id, UpdatePatientRequestDto request);
        Task<string?> DeletePatientAsync(Guid id);

    }
}
