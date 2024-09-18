using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class MedicalFileService : IMedicalFileService
    {
        private readonly IMedicalFileRepository medicalFileRepository;

        public MedicalFileService(IMedicalFileRepository medicalFileRepository)
        {
            this.medicalFileRepository = medicalFileRepository;
        }
        public async Task<MedicalFileDto> CreateMedicalFileAsync(CreateMedicalFileRequestDto request)
        {
            var medicalFile = new MedicalFile
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                FileName = request.FileName,
                FileType = request.FileType,
                FilePath = request.FilePath,
                DateUploaded = request.DateUploaded
            };

            await medicalFileRepository.CreateAsync(medicalFile);

            return new MedicalFileDto
            {
                Id = medicalFile.Id,
                PatientId = medicalFile.PatientId,
                FileName = medicalFile.FileName,
                FileType = medicalFile.FileType,
                FilePath = medicalFile.FilePath,
                DateUploaded = medicalFile.DateUploaded
            };
        }

        public async Task<IEnumerable<MedicalFileDto>> GetAllMedicalFilesAsync()
        {
            var medicalFiles = await medicalFileRepository.GetAllAsync();

            return medicalFiles.Select(medicalFile => new MedicalFileDto
            {
                Id = medicalFile.Id,
                PatientId = medicalFile.PatientId,
                FileName = medicalFile.FileName,
                FileType = medicalFile.FileType,
                FilePath = medicalFile.FilePath,
                DateUploaded = medicalFile.DateUploaded
            }).ToList();
        }

        public async Task<MedicalFileDto?> GetMedicalFileByIdAsync(Guid id)
        {
            var medicalFile = await medicalFileRepository.GetById(id);

            if(medicalFile == null)
            {
                return null;
            }

            return new MedicalFileDto
            {
                Id = medicalFile.Id,
                PatientId = medicalFile.PatientId,
                FileName = medicalFile.FileName,
                FileType = medicalFile.FileType,
                FilePath = medicalFile.FilePath,
                DateUploaded = medicalFile.DateUploaded
            };
        }

        public async Task<MedicalFileDto?> UpdateMedicalFileAsync(Guid id, UpdateMedicalFileRequestDto request)
        {
            var existingMedicalFile = await medicalFileRepository.GetById(id);
            if (existingMedicalFile == null) return null;
            
            existingMedicalFile.FileName = request.FileName;
            existingMedicalFile.FileType = request.FileType;
            existingMedicalFile.FilePath = request.FilePath;
            existingMedicalFile.DateUploaded = request.DateUploaded;

            var updatedMedicalFile = await medicalFileRepository.UpdateAsync(existingMedicalFile);

            if (updatedMedicalFile == null) return null;

            return new MedicalFileDto
            {
                Id = updatedMedicalFile.Id,
                PatientId = updatedMedicalFile.PatientId,
                FileName = updatedMedicalFile.FileName,
                FileType = updatedMedicalFile.FileType,
                FilePath = updatedMedicalFile.FilePath,
                DateUploaded = updatedMedicalFile.DateUploaded
            };
        }
        public async Task<MedicalFileDto> DeleteMedicalFileAsync(Guid id)
        {
            var medicalFile = await medicalFileRepository.DeleteAsync(id);
            if (medicalFile == null) return null;

            return new MedicalFileDto
            {
                Id = medicalFile.Id,
                PatientId = medicalFile.PatientId,
                FileName = medicalFile.FileName,
                FileType = medicalFile.FileType,
                FilePath = medicalFile.FilePath,
                DateUploaded = medicalFile.DateUploaded
            };

        }
    }
}
