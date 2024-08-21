using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MedNet.API.Repositories.Implementation
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ContactRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await dbContext.Contacts.AddAsync(contact);
            await dbContext.SaveChangesAsync();
            return contact;
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await dbContext.Contacts.ToListAsync();
        }

        public async Task<Contact?> GetById(Guid id)
        {
            return await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Contact?> UpdateAsync(Contact contact)
        {
            var existingContact = await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == contact.Id);

            if (existingContact != null)
            {
                dbContext.Entry(existingContact).CurrentValues.SetValues(contact);
                await dbContext.SaveChangesAsync();
                return contact;
            }

            return null;
        }

        public async Task<Contact?> DeleteAsync(Guid id)
        {
            var existingContact = await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);

            if (existingContact is null)
            {
                return null;
            }

            dbContext.Contacts.Remove(existingContact);
            await dbContext.SaveChangesAsync();
            return existingContact;
        }
    }
}
