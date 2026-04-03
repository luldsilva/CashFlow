using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Households
{
    public interface IHouseholdReadOnlyRepository
    {
        Task<Household?> GetByUserId(long userId);
    }
}
