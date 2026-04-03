using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.MonthlyClosures
{
    public interface IMonthlyClosuresUpdateOnlyRepository
    {
        Task<MonthlyClosure?> GetByMonth(long householdId, DateTime competenceDate);
        void Update(MonthlyClosure closure);
    }
}
