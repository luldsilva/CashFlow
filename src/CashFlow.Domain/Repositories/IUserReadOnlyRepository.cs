namespace CashFlow.Domain.Repositories
{
    public interface IUserReadOnlyRepository
    {
        Task<bool> ExistActiveUserWithEmail(string email);
        Task<Entities.User?> GetUserByEmail(string email);
    }
}
