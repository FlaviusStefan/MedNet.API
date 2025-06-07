﻿using MedNet.API.Models.DTO;

namespace MedNet.API.Services.Interface
{
    public interface IQualificationService
    {
        Task<QualificationDto> CreateQualificationAsync(CreateQualificationRequestDto request);
        Task<IEnumerable<QualificationDto>> GetAllQualificationsAsync();
        Task<QualificationDto?> GetQualificationByIdAsync(Guid id);
        Task<IEnumerable<QualificationDto>> GetQualificationsByDoctorIdAsync(Guid doctorId);
        Task<QualificationDto?> UpdateQualificationAsync(Guid id, UpdateQualificationRequestDto request);
        Task<QualificationDto?> DeleteQualificationAsync(Guid id);
    }
}