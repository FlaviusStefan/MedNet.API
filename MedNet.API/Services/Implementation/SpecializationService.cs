using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class SpecializationService : ISpecializationService
    {
        private readonly ISpecializationRepository specializationRepository;
        private readonly ILogger<SpecializationService> logger;

        public SpecializationService(ISpecializationRepository specializationRepository, ILogger<SpecializationService> logger)
        {
            this.specializationRepository = specializationRepository;
            this.logger = logger;
        }

        public async Task<SpecializationDto> CreateSpecializationAsync(CreateSpecializationRequestDto request)
        {
            logger.LogInformation("Creating specialization: {Name} - {Description}",
                request.Name, request.Description);

            var specialization = new Specialization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            await specializationRepository.CreateAsync(specialization);

            logger.LogInformation("Specialization {SpecializationId} created successfully - {Name}",
                specialization.Id, specialization.Name);

            return new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            };
        }

        public async Task<IEnumerable<SpecializationDto>> GetAllSpecializationsAsync()
        {
            logger.LogInformation("Retrieving all specializations");

            var specializations = await specializationRepository.GetAllAsync();

            var specializationList = specializations.Select(specialization => new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            }).ToList();

            logger.LogInformation("Retrieved {Count} specializations", specializationList.Count);

            return specializationList;
        }

        public async Task<SpecializationDto?> GetSpecializationByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving specialization with ID: {SpecializationId}", id);

            var specialization = await specializationRepository.GetById(id);
            if (specialization == null)
            {
                logger.LogWarning("Specialization not found with ID: {SpecializationId}", id);
                return null;
            }

            logger.LogInformation("Specialization {SpecializationId} retrieved - {Name}",
                specialization.Id, specialization.Name);

            return new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            };
        }

        public async Task<IEnumerable<SpecializationDto>> GetSpecializationsByDoctorIdAsync(Guid doctorId)
        {
            logger.LogInformation("Retrieving specializations for Doctor {DoctorId}", doctorId);

            var specializations = await specializationRepository.GetAllByDoctorIdAsync(doctorId);

            var specializationList = specializations.Select(specialization => new SpecializationDto
            {
                Id = specialization.Id,
                Name = specialization.Name,
                Description = specialization.Description
            }).ToList();

            logger.LogInformation("Retrieved {Count} specializations for Doctor {DoctorId}",
                specializationList.Count, doctorId);

            return specializationList;
        }

        public async Task<SpecializationDto?> UpdateSpecializationAsync(Guid id, UpdateSpecializationRequestDto request)
        {
            logger.LogInformation("Updating specialization with ID: {SpecializationId}", id);

            var existingSpecialization = await specializationRepository.GetById(id);

            if (existingSpecialization == null)
            {
                logger.LogWarning("Specialization not found for update with ID: {SpecializationId}", id);
                return null;
            }

            var oldName = existingSpecialization.Name;

            existingSpecialization.Name = request.Name;
            existingSpecialization.Description = request.Description;

            var updatedSpecialization = await specializationRepository.UpdateAsync(existingSpecialization);

            if (updatedSpecialization == null)
            {
                logger.LogError("Failed to update specialization with ID: {SpecializationId}", id);
                return null;
            }

            logger.LogInformation("Specialization {SpecializationId} updated successfully - Name: '{OldName}' → '{NewName}'",
                id, oldName, updatedSpecialization.Name);

            return new SpecializationDto
            {
                Id = updatedSpecialization.Id,
                Name = updatedSpecialization.Name,
                Description = updatedSpecialization.Description
            };
        }

        public async Task<string?> DeleteSpecializationAsync(Guid id)
        {
            logger.LogInformation("Deleting specialization with ID: {SpecializationId}", id);

            var specialization = await specializationRepository.DeleteAsync(id);
            if (specialization == null)
            {
                logger.LogWarning("Specialization not found for deletion with ID: {SpecializationId}", id);
                return null;
            }

            logger.LogInformation("Specialization {SpecializationId} deleted successfully - {Name}",
                specialization.Id, specialization.Name);

            return $"Specialization '{specialization.Name}' (ID: {specialization.Id}) deleted successfully!";
        }

        public async Task<Dictionary<Guid, string>> ValidateSpecializationsAsync(IEnumerable<Guid> specializationIds)
        {
            logger.LogInformation("Validating {Count} specialization IDs", specializationIds.Count());

            var allSpecializations = await GetAllSpecializationsAsync();
            var validSpecializations = allSpecializations
                .Where(s => specializationIds.Contains(s.Id))
                .ToDictionary(s => s.Id, s => s.Name);

            if (validSpecializations.Count != specializationIds.Count())
            {
                var invalidCount = specializationIds.Count() - validSpecializations.Count;
                logger.LogWarning("Specialization validation failed - {InvalidCount} invalid IDs out of {TotalCount}",
                    invalidCount, specializationIds.Count());
                throw new ArgumentException("One or more specialization IDs are invalid.");
            }

            logger.LogInformation("All {Count} specialization IDs validated successfully", validSpecializations.Count);

            return validSpecializations;
        }
    }
}