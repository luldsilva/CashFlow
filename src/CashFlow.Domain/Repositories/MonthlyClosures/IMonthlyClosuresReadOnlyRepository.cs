using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.MonthlyClosures
{
    public interface IMonthlyClosuresReadOnlyRepository
    {
        Task<MonthlyClosure?> GetByMonth(long householdId, DateTime competenceDate);
    }
}
