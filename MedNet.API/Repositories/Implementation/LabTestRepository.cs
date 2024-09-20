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
            await dbContext.SaveChangesAsync();

            return labTest;
        }

        public async Task<IEnumerable<LabTest>> GetAllAsync()
        {
            return await dbContext.LabTests.ToListAsync();
        }

        public async Task<LabTest?> GetById(Guid id)
        {
            return await dbContext.LabTests.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<LabTest?> UpdateAsync(LabTest labTest)
        {
            var existingTest= await dbContext.LabTests.FirstOrDefaultAsync(x => x.Id == labTest.Id);

            if (existingTest != null)
            {
                dbContext.Entry(existingTest).CurrentValues.SetValues(labTest);
                await dbContext.SaveChangesAsync();
                return existingTest;
            }

            return null;
        }

        public async Task<LabTest> DeleteAsync(Guid id)
        {
            var existingTest = await dbContext.LabTests.FirstOrDefaultAsync(x => x.Id == id);

            if (existingTest is null)
            {
                return null;
            }

            dbContext.LabTests.Remove(existingTest);
            await dbContext.SaveChangesAsync();
            return existingTest;
        }
    }
}
