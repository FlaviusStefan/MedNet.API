using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepository medicationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<MedicationService> logger;
        public MedicationService(IMedicationRepository medicationRepository, ILogger<MedicationService> logger, IUnitOfWork unitOfWork)
        {
            this.medicationRepository = medicationRepository;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }
        public async Task<MedicationDto> CreateMedicationAsync(CreateMedicationRequestDto request)
        {
            logger.LogInformation("Creating medication for Patient {PatientId}, Name: {Name}, Dosage: {Dosage}",
                request.PatientId, request.Name, request.Dosage);

            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                Name = request.Name,
                Dosage = request.Dosage,
                Frequency = request.Frequency
            };

            await medicationRepository.CreateAsync(medication);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Medication {MedicationId} created successfully for Patient {PatientId} - {Name}",
                medication.Id, medication.PatientId, medication.Name);

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
            logger.LogInformation("Retrieving all medications");

            var medications = await medicationRepository.GetAllAsync();

            var medicationList = medications.Select(medication => new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency
            }).ToList();

            logger.LogInformation("Retrieved {Count} medications", medicationList.Count);

            return medicationList;
        }

        public async Task<MedicationDto?> GetMedicationByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving medication with ID: {MedicationId}", id);

            var medication = await medicationRepository.GetById(id);
            if (medication is null)
            {
                logger.LogWarning("Medication not found with ID: {MedicationId}", id);
                return null;
            }

            logger.LogInformation("Medication {MedicationId} retrieved - Patient: {PatientId}, Name: {Name}",
                medication.Id, medication.PatientId, medication.Name);

            return new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency
            };
        }

        public async Task<IEnumerable<MedicationDto>> GetMedicationsByPatientIdAsync(Guid patientId)
        {
            logger.LogInformation("Retrieving medications for Patient {PatientId}", patientId);

            var medications = await medicationRepository.GetAllByPatientIdAsync(patientId);

            var medicationList = medications.Select(medication => new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency
            }).ToList();

            logger.LogInformation("Retrieved {Count} medications for Patient {PatientId}",
                medicationList.Count, patientId);

            return medicationList;
        }

        public async Task<MedicationDto> UpdateMedicationAsync(Guid id, UpdateMedicationRequestDto request)
        {
            logger.LogInformation("Updating medication with ID: {MedicationId}", id);

            var existingMedication = await medicationRepository.GetById(id);
            if (existingMedication is null)
            {
                logger.LogWarning("Medication not found for update with ID: {MedicationId}", id);
                return null;
            }

            var oldName = existingMedication.Name;
            var oldDosage = existingMedication.Dosage;

            var medicationToUpdate = new Medication
            {
                Id = id,
                PatientId = existingMedication.PatientId,
                Name = request.Name,
                Dosage = request.Dosage,
                Frequency = request.Frequency
            };

            var updatedMedication = await medicationRepository.UpdateAsync(medicationToUpdate);

            if (updatedMedication is null)
            {
                logger.LogError("Failed to update medication with ID: {MedicationId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Medication {MedicationId} updated successfully - Name: '{OldName}' → '{NewName}', Dosage: '{OldDosage}' → '{NewDosage}'",
                id, oldName, updatedMedication.Name, oldDosage, updatedMedication.Dosage);

            return new MedicationDto
            {
                Id = updatedMedication.Id,
                PatientId = updatedMedication.PatientId,
                Name = updatedMedication.Name,
                Dosage = updatedMedication.Dosage,
                Frequency = updatedMedication.Frequency
            };
        }

        public async Task<string?> DeleteMedicationAsync(Guid id)
        {
            logger.LogInformation("Deleting medication with ID: {MedicationId}", id);

            var medication = await medicationRepository.DeleteAsync(id);
            if (medication is null)
            {
                logger.LogWarning("Medication not found for deletion with ID: {MedicationId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Medication {MedicationId} deleted successfully - Patient: {PatientId}, Name: {Name}",
                medication.Id, medication.PatientId, medication.Name);

            return $"Medication '{medication.Name}' deleted successfully!";
        }
    }
}