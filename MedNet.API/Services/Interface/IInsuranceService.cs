using MedNet.API.Models;
using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IInsuranceService
    {
        Task<InsuranceDto> CreateInsuranceAsync(CreateInsuranceRequestDto request);
        Task<IEnumerable<InsuranceDto>> GetAllInsurancesAsync();
        Task<InsuranceDto?> GetInsuranceByIdAsync(Guid id);
        Task<IEnumerable<InsuranceDto>> GetInsurancesByPatientIdAsync(Guid patientId);
        Task<InsuranceDto> UpdateInsuranceAsync(Guid id, UpdateInsuranceRequestDto request);
        Task<InsuranceDto> DeleteInsuranceAsync(Guid id);
    }
}