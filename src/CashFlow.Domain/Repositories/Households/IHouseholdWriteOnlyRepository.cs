using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Households
{
    public interface IHouseholdWriteOnlyRepository
    {
        Task Add(Household household);
        Task Delete(long id);
    }
}
