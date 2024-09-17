using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IMedicationService
    {
        Task<MedicationDto> CreateMedicationAsync(CreateMedicationRequestDto request);
        Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync();
        Task<MedicationDto?> GetMedicationByIdAsync(Guid id);
        Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationRequestDto request);
        Task<MedicationDto> DeleteMedicationAsync(Guid id);
    }
}
