using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Responses
{
    public class ResponseIncomeSource
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IncomeSourceType Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsRecurring { get; set; }
        public HouseholdIncomeFrequency Frequency { get; set; }
        public byte? ExpectedDayOfMonth { get; set; }
    }
}
