using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Responses
{
    public class ResponsePreparedIncomeSource
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsRecurring { get; set; }
        public HouseholdIncomeFrequency Frequency { get; set; }
        public bool RequiresReview { get; set; }
    }
}
