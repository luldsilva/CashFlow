using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    public class UserRepository : IUserReadOnlyRepository, IUserWriteOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public UserRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<bool> ExistActiveUserWithEmail(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email.Equals(email));
        }
    }
}
