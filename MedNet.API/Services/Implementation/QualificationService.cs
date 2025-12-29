using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Implementation;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class QualificationService : IQualificationService
    {
        private readonly IQualificationRepository qualificationRepository;

        public QualificationService(IQualificationRepository qualificationRepository)
        {
            this.qualificationRepository = qualificationRepository;
        }

        public async Task<QualificationDto> CreateQualificationAsync(CreateQualificationRequestDto request)
        {
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
            var qualifications = await qualificationRepository.GetAllAsync();

            return qualifications.Select(qualification => new QualificationDto
            {
                Id = qualification.Id,
                DoctorId = qualification.DoctorId,
                Degree = qualification.Degree,
                Institution = qualification.Institution,
                StudiedYears = qualification.StudiedYears,
                YearOfCompletion = qualification.YearOfCompletion
            }).ToList();
        }

        public async Task<QualificationDto?> GetQualificationByIdAsync(Guid id)
        {
            var qualification = await qualificationRepository.GetById(id);
            if (qualification == null)
            {
                return null;
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

        public async Task<IEnumerable<QualificationDto>> GetQualificationsByDoctorIdAsync(Guid doctorId)
        {
            var qualifications = await qualificationRepository.GetAllByDoctorIdAsync(doctorId);
            return qualifications.Select(qualification => new QualificationDto
            {
                Id = qualification.Id,
                DoctorId = qualification.DoctorId,
                Degree = qualification.Degree,
                Institution = qualification.Institution,
                StudiedYears = qualification.StudiedYears,
                YearOfCompletion = qualification.YearOfCompletion
            }).ToList();
        }

        public async Task<QualificationDto?> UpdateQualificationAsync(Guid id, UpdateQualificationRequestDto request)
        {
            var existingQualification = await qualificationRepository.GetById(id);

            if (existingQualification == null) return null;

            existingQualification.Degree = request.Degree;
            existingQualification.Institution = request.Institution;


            var updatedQualification = await qualificationRepository.UpdateAsync(existingQualification);

            if (updatedQualification == null) return null;

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

        public async Task<QualificationDto?> DeleteQualificationAsync(Guid id)
        {
            var qualification = await qualificationRepository.DeleteAsync(id);
            if (qualification == null) return null;

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
        public async Task<IEnumerable<TDto>> GetQualificationsByDoctorIdAsync<TDto>(Guid doctorId, Func<Qualification, TDto> selector)
        {
            var qualifications = await qualificationRepository.GetAllByDoctorIdAsync(doctorId);
            return qualifications.Select(selector).ToList();
        }
    }
}
