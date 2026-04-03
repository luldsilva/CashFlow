using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.FinancialObligations
{
    public interface IFinancialObligationsUpdateOnlyRepository
    {
        Task<FinancialObligation?> GetById(long householdId, long id);
        void Update(FinancialObligation obligation);
    }
}
