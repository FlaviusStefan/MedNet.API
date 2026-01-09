using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class MedicalFileService : IMedicalFileService
    {
        private readonly IMedicalFileRepository medicalFileRepository;
        private readonly ILogger<MedicalFileService> logger;

        public MedicalFileService(IMedicalFileRepository medicalFileRepository, ILogger<MedicalFileService> logger)
        {
            this.medicalFileRepository = medicalFileRepository;
            this.logger = logger;
        }
        public async Task<MedicalFileDto> CreateMedicalFileAsync(CreateMedicalFileRequestDto request)
        {
            logger.LogInformation("Creating medical file for Patient {PatientId}, File: {FileName}, Type: {FileType}",
                request.PatientId, request.FileName, request.FileType);

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

            logger.LogInformation("Medical file {FileId} created successfully for Patient {PatientId} - {FileName}",
                medicalFile.Id, medicalFile.PatientId, medicalFile.FileName);

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
            logger.LogInformation("Retrieving all medical files");

            var medicalFiles = await medicalFileRepository.GetAllAsync();

            var fileList = medicalFiles.Select(medicalFile => new MedicalFileDto
            {
                Id = medicalFile.Id,
                PatientId = medicalFile.PatientId,
                FileName = medicalFile.FileName,
                FileType = medicalFile.FileType,
                FilePath = medicalFile.FilePath,
                DateUploaded = medicalFile.DateUploaded
            }).ToList();

            logger.LogInformation("Retrieved {Count} medical files", fileList.Count);

            return fileList;
        }

        public async Task<MedicalFileDto?> GetMedicalFileByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving medical file with ID: {FileId}", id);

            var medicalFile = await medicalFileRepository.GetById(id);

            if (medicalFile == null)
            {
                logger.LogWarning("Medical file not found with ID: {FileId}", id);
                return null;
            }

            logger.LogInformation("Medical file {FileId} retrieved - Patient: {PatientId}, File: {FileName}",
                medicalFile.Id, medicalFile.PatientId, medicalFile.FileName);

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

        public async Task<IEnumerable<MedicalFileDto>> GetMedicalFilesByPatientIdAsync(Guid patientId)
        {
            logger.LogInformation("Retrieving medical files for Patient {PatientId}", patientId);

            var medicalFiles = await medicalFileRepository.GetAllByPatientIdAsync(patientId);

            var fileList = medicalFiles.Select(mf => new MedicalFileDto
            {
                Id = mf.Id,
                PatientId = mf.PatientId,
                FileName = mf.FileName,
                FileType = mf.FileType,
                FilePath = mf.FilePath,
                DateUploaded = mf.DateUploaded
            }).ToList();

            logger.LogInformation("Retrieved {Count} medical files for Patient {PatientId}",
                fileList.Count, patientId);

            return fileList;
        }


        public async Task<MedicalFileDto?> UpdateMedicalFileAsync(Guid id, UpdateMedicalFileRequestDto request)
        {
            logger.LogInformation("Updating medical file with ID: {FileId}", id);

            var existingMedicalFile = await medicalFileRepository.GetById(id);
            if (existingMedicalFile == null)
            {
                logger.LogWarning("Medical file not found for update with ID: {FileId}", id);
                return null;
            }

            var oldFileName = existingMedicalFile.FileName;

            existingMedicalFile.FileName = request.FileName;
            existingMedicalFile.FileType = request.FileType;
            existingMedicalFile.FilePath = request.FilePath;
            existingMedicalFile.DateUploaded = request.DateUploaded;

            var updatedMedicalFile = await medicalFileRepository.UpdateAsync(existingMedicalFile);

            if (updatedMedicalFile == null)
            {
                logger.LogError("Failed to update medical file with ID: {FileId}", id);
                return null;
            }

            logger.LogInformation("Medical file {FileId} updated successfully - FileName: '{OldName}' → '{NewName}'",
                id, oldFileName, updatedMedicalFile.FileName);

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
        public async Task<string?> DeleteMedicalFileAsync(Guid id)
        {
            logger.LogInformation("Deleting medical file with ID: {FileId}", id);

            var medicalFile = await medicalFileRepository.DeleteAsync(id);
            if (medicalFile == null)
            {
                logger.LogWarning("Medical file not found for deletion with ID: {FileId}", id);
                return null;
            }

            logger.LogInformation("Medical file {FileId} deleted successfully - Patient: {PatientId}, File: {FileName}",
                medicalFile.Id, medicalFile.PatientId, medicalFile.FileName);

            return $"Medical file '{medicalFile.FileName}' deleted successfully!";
        }
    }
}
