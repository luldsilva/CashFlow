using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entities
{
    public class IncomeSource
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IncomeSourceType Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsRecurring { get; set; }
        public HouseholdIncomeFrequency Frequency { get; set; }
        public byte? ExpectedDayOfMonth { get; set; }

        public long HouseholdId { get; set; }
        public Household Household { get; set; } = default!;
    }
}
