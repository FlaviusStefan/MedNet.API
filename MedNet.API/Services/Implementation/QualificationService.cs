using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class QualificationService : IQualificationService
    {
        private readonly IQualificationRepository qualificationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<QualificationService> logger;

        public QualificationService(IQualificationRepository qualificationRepository, ILogger<QualificationService> logger, IUnitOfWork unitOfWork)
        {
            this.qualificationRepository = qualificationRepository;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationRequestDto request, bool autoSave = true)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.DoctorId == Guid.Empty)
            {
                logger.LogWarning("CreateQualificationAsync called with empty DoctorId");
                throw new ArgumentException("DoctorId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Degree) || string.IsNullOrWhiteSpace(request.Institution))
            {
                logger.LogWarning("CreateQualificationAsync called with invalid request values for Doctor {DoctorId}", request.DoctorId);
                throw new ArgumentException("Degree and Institution are required.");
            }

            if (request.StudiedYears < 1 || request.StudiedYears > 15)
            {
                logger.LogWarning("CreateQualificationAsync called with invalid StudiedYears for Doctor {DoctorId}: {StudiedYears}", request.DoctorId, request.StudiedYears);
                throw new ArgumentException("StudiedYears must be between 1 and 15.");
            }

            if (request.YearOfCompletion < 1950 || request.YearOfCompletion > 2100)
            {
                logger.LogWarning("CreateQualificationAsync called with invalid YearOfCompletion for Doctor {DoctorId}: {YearOfCompletion}", request.DoctorId, request.YearOfCompletion);
                throw new ArgumentException("YearOfCompletion must be between 1950 and 2100.");
            }

            logger.LogInformation("Creating qualification for Doctor {DoctorId}: {Degree} from {Institution}",
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

            if (autoSave)
            {
                await unitOfWork.SaveChangesAsync();
                logger.LogInformation("Qualification {QualificationId} created and saved for Doctor {DoctorId} - {Degree} from {Institution}",
                    qualification.Id, qualification.DoctorId, qualification.Degree, qualification.Institution);
            }
            else
            {
                logger.LogDebug("Qualification {QualificationId} tracked (deferred save) for Doctor {DoctorId}",
                    qualification.Id, qualification.DoctorId);
            }

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

            logger.LogInformation("Qualification {QualificationId} retrieved - Doctor: {DoctorId}, {Degree} from {Institution}",
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
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Degree) || string.IsNullOrWhiteSpace(request.Institution))
            {
                logger.LogWarning("UpdateQualificationAsync called with invalid request for id {Id}", id);
                throw new ArgumentException("Degree and Institution are required.");
            }

            if (request.StudiedYears < 1 || request.StudiedYears > 15)
            {
                logger.LogWarning("UpdateQualificationAsync called with invalid StudiedYears for id {Id}: {StudiedYears}", id, request.StudiedYears);
                throw new ArgumentException("StudiedYears must be between 1 and 15.");
            }

            if (request.YearOfCompletion < 1950 || request.YearOfCompletion > 2100)
            {
                logger.LogWarning("UpdateQualificationAsync called with invalid YearOfCompletion for id {Id}: {YearOfCompletion}", id, request.YearOfCompletion);
                throw new ArgumentException("YearOfCompletion must be between 1950 and 2100.");
            }

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
            existingQualification.StudiedYears = request.StudiedYears;
            existingQualification.YearOfCompletion = request.YearOfCompletion;

            var updatedQualification = await qualificationRepository.UpdateAsync(existingQualification);

            if (updatedQualification == null)
            {
                logger.LogError("Failed to update qualification with ID: {QualificationId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

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

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Qualification {QualificationId} deleted successfully - Doctor: {DoctorId}, {Degree} from {Institution}",
                qualification.Id, qualification.DoctorId, qualification.Degree, qualification.Institution);

            return $"Qualification '{qualification.Degree}' from {qualification.Institution} (ID: {qualification.Id}) deleted successfully!";
        }
    }
}