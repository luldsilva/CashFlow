using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.FinancialObligations
{
    public interface IFinancialObligationsWriteOnlyRepository
    {
        Task Add(FinancialObligation obligation);
        Task Delete(long id);
    }
}
