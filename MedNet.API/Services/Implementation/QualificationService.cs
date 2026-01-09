using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class QualificationService : IQualificationService
    {
        private readonly IQualificationRepository qualificationRepository;
        private readonly ILogger<QualificationService> logger;

        public QualificationService(IQualificationRepository qualificationRepository, ILogger<QualificationService> logger)
        {
            this.qualificationRepository = qualificationRepository;
            this.logger = logger;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationRequestDto request)
        {
            logger.LogInformation("Creating qualification for Doctor {DoctorId}, Degree: {Degree}, Institution: {Institution}",
                request.DoctorId, request.Degree, request.Institution);

            var qualification = new Qualification
            {
                Id = Guid.NewGuid(),
                DoctorId = request.DoctorId,
                Degree = request.Degree,
                Institution = request.Institution,
                StudiedYears = request.StudiedYears,
                YearOfCompletion = request.YearOfCompletion
            };

            await qualificationRepository.CreateAsync(qualification);

            logger.LogInformation("Qualification {QualificationId} created successfully for Doctor {DoctorId} - {Degree} from {Institution}",
                qualification.Id, qualification.DoctorId, qualification.Degree, qualification.Institution);

            return new QualificationDto
            {
                Id = qualification.Id,
                DoctorId = qualification.DoctorId,
                Degree = qualification.Degree,
                Institution = qualification.Institution,
                StudiedYears = qualification.StudiedYears,
                YearOfCompletion = qualification.YearOfCompletion
            };
        }

        public async Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync()
        {
            logger.LogInformation("Retrieving all qualifications");

            var qualifications = await qualificationRepository.GetAllAsync();

            var qualificationList = qualifications.Select(qualification => new QualificationDto
            {
                Id = qualification.Id,
                DoctorId = qualification.DoctorId,
                Degree = qualification.Degree,
                Institution = qualification.Institution,
                StudiedYears = qualification.StudiedYears,
                YearOfCompletion = qualification.YearOfCompletion
            }).ToList();

            logger.LogInformation("Retrieved {Count} qualifications", qualificationList.Count);

            return qualificationList;
        }

        public async Task<QualificationDto?> GetQualificationByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving qualification with ID: {QualificationId}", id);

            var qualification = await qualificationRepository.GetById(id);
            if (qualification == null)
            {
                logger.LogWarning("Qualification not found with ID: {QualificationId}", id);
                return null;
            }

            logger.LogInformation("Qualification {QualificationId} retrieved - Doctor: {DoctorId}, Degree: {Degree}",
                qualification.Id, qualification.DoctorId, qualification.Degree);

            return new QualificationDto
            {
                Id = qualification.Id,
                DoctorId = qualification.DoctorId,
                Degree = qualification.Degree,
                Institution = qualification.Institution,
                StudiedYears = qualification.StudiedYears,
                YearOfCompletion = qualification.YearOfCompletion
            };
        }

        public async Task<IEnumerable<QualificationDto>> GetQualificationsByDoctorIdAsync(Guid doctorId)
        {
            logger.LogInformation("Retrieving qualifications for Doctor {DoctorId}", doctorId);

            var qualifications = await qualificationRepository.GetAllByDoctorIdAsync(doctorId);

            var qualificationList = qualifications.Select(qualification => new QualificationDto
            {
                Id = qualification.Id,
                DoctorId = qualification.DoctorId,
                Degree = qualification.Degree,
                Institution = qualification.Institution,
                StudiedYears = qualification.StudiedYears,
                YearOfCompletion = qualification.YearOfCompletion
            }).ToList();

            logger.LogInformation("Retrieved {Count} qualifications for Doctor {DoctorId}",
                qualificationList.Count, doctorId);

            return qualificationList;
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(Guid id, UpdateQualificationRequestDto request)
        {
            logger.LogInformation("Updating qualification with ID: {QualificationId}", id);

            var existingQualification = await qualificationRepository.GetById(id);

            if (existingQualification == null)
            {
                logger.LogWarning("Qualification not found for update with ID: {QualificationId}", id);
                return null;
            }

            var oldDegree = existingQualification.Degree;
            var oldInstitution = existingQualification.Institution;

            existingQualification.Degree = request.Degree;
            existingQualification.Institution = request.Institution;

            var updatedQualification = await qualificationRepository.UpdateAsync(existingQualification);

            if (updatedQualification == null)
            {
                logger.LogError("Failed to update qualification with ID: {QualificationId}", id);
                return null;
            }

            logger.LogInformation("Qualification {QualificationId} updated successfully - Degree: '{OldDegree}' → '{NewDegree}', Institution: '{OldInstitution}' → '{NewInstitution}'",
                id, oldDegree, updatedQualification.Degree, oldInstitution, updatedQualification.Institution);

            return new QualificationDto
            {
                Id = updatedQualification.Id,
                DoctorId = updatedQualification.DoctorId,
                Degree = updatedQualification.Degree,
                Institution = updatedQualification.Institution,
                StudiedYears = updatedQualification.StudiedYears,
                YearOfCompletion = updatedQualification.YearOfCompletion
            };
        }

        public async Task<string?> DeleteQualificationAsync(Guid id)
        {
            logger.LogInformation("Deleting qualification with ID: {QualificationId}", id);

            var qualification = await qualificationRepository.DeleteAsync(id);
            if (qualification == null)
            {
                logger.LogWarning("Qualification not found for deletion with ID: {QualificationId}", id);
                return null;
            }

            logger.LogInformation("Qualification {QualificationId} deleted successfully - Doctor: {DoctorId}, Degree: {Degree} from {Institution}",
                qualification.Id, qualification.DoctorId, qualification.Degree, qualification.Institution);

            return $"Qualification '{qualification.Degree}' from {qualification.Institution} deleted successfully!";
        }

        public async Task<IEnumerable<TDto>> GetQualificationsByDoctorIdAsync<TDto>(Guid doctorId, Func<Qualification, TDto> selector)
        {
            logger.LogDebug("Retrieving qualifications for Doctor {DoctorId} with custom selector", doctorId);

            var qualifications = await qualificationRepository.GetAllByDoctorIdAsync(doctorId);

            var result = qualifications.Select(selector).ToList();

            logger.LogDebug("Retrieved and transformed {Count} qualifications for Doctor {DoctorId}",
                result.Count, doctorId);

            return result;
        }
    }
}