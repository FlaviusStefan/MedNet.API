using MedNet.API.Data;
using MedNet.API.Models;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Repositories.Implementation
{
    public class LabTestRepository : ILabTestRepository
    {
        private readonly ApplicationDbContext dbContext;

        public LabTestRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<LabTest> CreateAsync(LabTest labTest)
        {
            await dbContext.LabTests.AddAsync(labTest);
            return labTest;
        }

        public async Task<IEnumerable<LabTest>> GetAllAsync()
        {
            return await dbContext.LabTests
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LabTest?> GetById(Guid id)
        {
            return await dbContext.LabTests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<LabTest?> UpdateAsync(LabTest labTest)
        {
            var existingTest= await dbContext.LabTests
                .FirstOrDefaultAsync(x => x.Id == labTest.Id);

            if (existingTest is null)
            {
                return null;
            }

            existingTest.TestName = labTest.TestName;
            existingTest.Result = labTest.Result;
            existingTest.Units = labTest.Units;
            existingTest.ReferenceRange = labTest.ReferenceRange;

            return existingTest;
        }

        public async Task<LabTest> DeleteAsync(Guid id)
        {
            var existingTest = await dbContext.LabTests
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingTest is null)
            {
                return null;
            }

            dbContext.LabTests.Remove(existingTest);
            return existingTest;
        }
    }
}