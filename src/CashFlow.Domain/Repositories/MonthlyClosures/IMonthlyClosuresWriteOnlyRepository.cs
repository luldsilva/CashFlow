using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.MonthlyClosures
{
    public interface IMonthlyClosuresWriteOnlyRepository
    {
        Task Add(MonthlyClosure closure);
    }
}
