using MedNet.API.Exceptions;
using MedNet.API.Models;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;
using Microsoft.Extensions.Logging;

namespace MedNet.API.Services.Implementation
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository insuranceRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<InsuranceService> logger;

        public InsuranceService(IInsuranceRepository insuranceRepository, ILogger<InsuranceService> logger, IUnitOfWork unitOfWork)
        {
            this.insuranceRepository = insuranceRepository;
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public async Task<InsuranceDto> CreateInsuranceAsync(CreateInsuranceRequestDto request)
        {
            if (!request.PatientId.HasValue)
            {
                logger.LogWarning("Insurance creation failed - PatientId is null");
                throw new InvalidOperationException("PatientId cannot be null.");
            }

            logger.LogInformation("Creating insurance for Patient {PatientId}, Provider: {Provider}, Policy: {PolicyNumber}", 
                request.PatientId.Value, request.Provider, request.PolicyNumber);

            var insurance = new Insurance
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId.Value,
                Provider = request.Provider,
                PolicyNumber = request.PolicyNumber,
                CoverageStartDate = request.CoverageStartDate,
                CoverageEndDate = request.CoverageEndDate
            };

            await insuranceRepository.CreateAsync(insurance);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Insurance {InsuranceId} created successfully for Patient {PatientId} - Provider: {Provider}", 
                insurance.Id, insurance.PatientId, insurance.Provider);

            return new InsuranceDto
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                Provider = insurance.Provider,
                PolicyNumber = insurance.PolicyNumber,
                CoverageStartDate = insurance.CoverageStartDate,
                CoverageEndDate = insurance.CoverageEndDate
            };
        }

        public async Task<IEnumerable<InsuranceDto>> GetAllInsurancesAsync()
        {
            logger.LogInformation("Retrieving all insurances");

            var insurances = await insuranceRepository.GetAllAsync();

            var insuranceList = insurances.Select(insurance => new InsuranceDto
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                Provider = insurance.Provider,
                PolicyNumber = insurance.PolicyNumber,
                CoverageStartDate = insurance.CoverageStartDate,
                CoverageEndDate = insurance.CoverageEndDate
            }).ToList();

            logger.LogInformation("Retrieved {Count} insurances", insuranceList.Count);

            return insuranceList;
        }

        public async Task<InsuranceDto?> GetInsuranceByIdAsync(Guid id)
        {
            logger.LogInformation("Retrieving insurance with ID: {InsuranceId}", id);

            var insurance = await insuranceRepository.GetById(id);
            if(insurance is null)
            {
                logger.LogWarning("Insurance not found with ID: {InsuranceId}", id);
                return null;
            }

            logger.LogInformation("Insurance {InsuranceId} retrieved - Patient: {PatientId}, Provider: {Provider}", 
                insurance.Id, insurance.PatientId, insurance.Provider);

            return new InsuranceDto
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                Provider = insurance.Provider,
                PolicyNumber = insurance.PolicyNumber,
                CoverageStartDate = insurance.CoverageStartDate,
                CoverageEndDate = insurance.CoverageEndDate
            };
        }

        public async Task<IEnumerable<InsuranceDto>> GetInsurancesByPatientIdAsync(Guid patientId)
        {
            logger.LogInformation("Retrieving insurances for Patient {PatientId}", patientId);

            var insurances = await insuranceRepository.GetAllByPatientIdAsync(patientId);

            var insuranceList = insurances.Select(insurance => new InsuranceDto
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                Provider = insurance.Provider,
                PolicyNumber = insurance.PolicyNumber,
                CoverageStartDate = insurance.CoverageStartDate,
                CoverageEndDate = insurance.CoverageEndDate
            }).ToList();

            logger.LogInformation("Retrieved {Count} insurances for Patient {PatientId}", 
                insuranceList.Count, patientId);

            return insuranceList;
        }

        public async Task<InsuranceDto> UpdateInsuranceAsync(Guid id, UpdateInsuranceRequestDto request)
        {
            logger.LogInformation("Updating insurance with ID: {InsuranceId}", id);

            var existingInsurance = await insuranceRepository.GetById(id);
            if (existingInsurance is null)
            {
                logger.LogWarning("Insurance not found for update with ID: {InsuranceId}", id);
                return null;
            }

            var oldProvider = existingInsurance.Provider;
            var oldPolicyNumber = existingInsurance.PolicyNumber;

            var insuranceToUpdate = new Insurance
            {
                Id = id,
                PatientId = existingInsurance.PatientId,
                Provider = request.Provider,
                PolicyNumber = request.PolicyNumber,
                CoverageStartDate = request.CoverageStartDate,
                CoverageEndDate = request.CoverageEndDate
            };

            var updatedInsurance = await insuranceRepository.UpdateAsync(insuranceToUpdate);

            if (updatedInsurance is null)
            {
                logger.LogError("Failed to update insurance with ID: {InsuranceId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Insurance {InsuranceId} updated successfully - Provider: '{OldProvider}' → '{NewProvider}', Policy: '{OldPolicy}' → '{NewPolicy}'",
                id, oldProvider, updatedInsurance.Provider, oldPolicyNumber, updatedInsurance.PolicyNumber);

            return new InsuranceDto
            {
                Id = updatedInsurance.Id,
                PatientId = updatedInsurance.PatientId,
                Provider = updatedInsurance.Provider,
                PolicyNumber = updatedInsurance.PolicyNumber,
                CoverageStartDate = updatedInsurance.CoverageStartDate,
                CoverageEndDate = updatedInsurance.CoverageEndDate
            };
        }

        public async Task<string?> DeleteInsuranceAsync(Guid id)
        {
            logger.LogInformation("Deleting insurance with ID: {InsuranceId}", id);

            var insurance = await insuranceRepository.DeleteAsync(id);
            if (insurance is null)
            {
                logger.LogWarning("Insurance not found for deletion with ID: {InsuranceId}", id);
                return null;
            }

            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Insurance {InsuranceId} deleted successfully - Patient: {PatientId}, Provider: {Provider}", 
                insurance.Id, insurance.PatientId, insurance.Provider);

            return $"Insurance with ID {insurance.Id} deleted successfully!";


        }
    }
}
