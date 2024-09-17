using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository medicationRepository;

        public MedicationService(IMedicationRepository medicationRepository)
        {
            this.medicationRepository = medicationRepository;
        }
        public async Task<MedicationDto> CreateMedicationAsync(CreateMedicationRequestDto request)
        {
            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                Name = request.Name,
                Dosage = request.Dosage,
                Frequency = request.Frequency
            };

            await medicationRepository.CreateAsync(medication);

            return new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency
            };
        }

        public async Task<IEnumerable<MedicationDto>> GetAllMedicationsAsync()
        {
            var medications = await medicationRepository.GetAllAsync();

            return medications.Select(medication => new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Name,
                Frequency = medication.Frequency
            }).ToList();
        }

        public async Task<MedicationDto?> GetMedicationByIdAsync(Guid id)
        {
            var medication = await medicationRepository.GetById(id);
            if(medication == null)
            {
                return null;
            }

            return new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency
            };
        }

        public async Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationRequestDto request)
        {
            var existingMedication = await medicationRepository.GetById(id);
            if (existingMedication == null) return null;

            existingMedication.Name = request.Name;
            existingMedication.Dosage = request.Dosage;
            existingMedication.Frequency = request.Frequency;

            var updatedMedication = await medicationRepository.UpdateAsync(existingMedication);

            if (updatedMedication == null) return null;

            return new MedicationDto
            {
                Id = updatedMedication.Id,
                PatientId = updatedMedication.PatientId,
                Name = updatedMedication.Name,
                Dosage = updatedMedication.Dosage,
                Frequency = updatedMedication.Frequency
            };
        }
        public async Task<MedicationDto> DeleteMedicationAsync(Guid id)
        {
            var medication = await medicationRepository.DeleteAsync(id);
            if (medication == null) return null;

            return new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency
            };
        }
    }
}
