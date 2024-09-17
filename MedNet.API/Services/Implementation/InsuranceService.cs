using MedNet.API.Models;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Interface;

namespace MedNet.API.Services.Implementation
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository insuranceRepository;

        public InsuranceService(IInsuranceRepository insuranceRepository)
        {
            this.insuranceRepository = insuranceRepository;
        }

        public async Task<InsuranceDto> CreateInsuranceAsync(CreateInsuranceRequestDto request)
        {
            var insurance = new Insurance
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                Provider = request.Provider,
                PolicyNumber = request.PolicyNumber,
                CoverageStartDate = request.CoverageStartDate,
                CoverageEndDate = request.CoverageEndDate
            };

            await insuranceRepository.CreateAsync(insurance);

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
            var insurances = await insuranceRepository.GetAllAsync();

            return insurances.Select(insurance => new InsuranceDto
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                Provider = insurance.Provider,
                PolicyNumber = insurance.PolicyNumber,
                CoverageStartDate = insurance.CoverageStartDate,
                CoverageEndDate = insurance.CoverageEndDate
            }).ToList();
        }

        public async Task<InsuranceDto?> GetInsuranceByIdAsync(Guid id)
        {
            var insurance = await insuranceRepository.GetById(id);
            if(insurance == null)
            {
                return null;
            }

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

        public async Task<InsuranceDto> UpdateInsuranceAsync(Guid id, UpdateInsuranceRequestDto request)
        {
            var existingInsurance = await insuranceRepository.GetById(id);
            if (existingInsurance == null) return null;

            existingInsurance.Provider = request.Provider;
            existingInsurance.PolicyNumber = request.PolicyNumber;
            existingInsurance.CoverageStartDate = request.CoverageStartDate;
            existingInsurance.CoverageEndDate = request.CoverageEndDate;

            var updatedInsurance = await insuranceRepository.UpdateAsync(existingInsurance);

            if (updatedInsurance == null) return null;

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

        public async Task<InsuranceDto> DeleteInsuranceAsync(Guid id)
        {
            var insurance = await insuranceRepository.DeleteAsync(id);
            if (insurance == null) return null;

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
    }
}
