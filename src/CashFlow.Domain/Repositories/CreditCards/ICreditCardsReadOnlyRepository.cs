using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.CreditCards
{
    public interface ICreditCardsReadOnlyRepository
    {
        Task<List<CreditCard>> GetAll(long householdId);
        Task<CreditCard?> GetById(long householdId, long id);
    }
}
