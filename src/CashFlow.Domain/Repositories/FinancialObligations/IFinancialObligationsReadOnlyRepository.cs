using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.FinancialObligations
{
    public interface IFinancialObligationsReadOnlyRepository
    {
        Task<List<FinancialObligation>> GetAllByCompetence(long householdId, DateTime competenceDate);
        Task<FinancialObligation?> GetById(long householdId, long id);
    }
}
