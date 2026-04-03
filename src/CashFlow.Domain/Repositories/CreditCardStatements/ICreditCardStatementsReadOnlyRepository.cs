using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.CreditCardStatements
{
    public interface ICreditCardStatementsReadOnlyRepository
    {
        Task<List<CreditCardStatement>> GetByMonth(long householdId, DateTime competenceDate);
    }
}
