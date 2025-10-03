using MyMuscleCars.Models;
using MyMuscleCars.Data;

namespace MyMuscleCars.Services
{
    public interface IContactService
    {
        Task<bool> SubmitContactMessageAsync(ContactMessage message);
    }

    public class ContactService : IContactService
    {
        private readonly AppDbContext _dbContext;

        public ContactService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SubmitContactMessageAsync(ContactMessage message)
        {
            try
            {
                _dbContext.ContactMessages.Add(message);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
