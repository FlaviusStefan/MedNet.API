using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IMedicationService
    {
        Task<MedicationDto> CreateMedicationAsync(CreateMedicationRequestDto request);
        Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync();
        Task<MedicationDto?> GetMedicationByIdAsync(Guid id);
        Task<IEnumerable<MedicationDto>> GetMedicationsByPatientIdAsync(Guid patientId);
        Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationRequestDto request);
        Task<string?> DeleteMedicationAsync(Guid id);
    }
}
