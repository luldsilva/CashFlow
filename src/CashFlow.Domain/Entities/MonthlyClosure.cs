namespace CashFlow.Domain.Entities
{
    public class MonthlyClosure
    {
        public long Id { get; set; }
        public DateTime CompetenceDate { get; set; }
        public decimal PlannedIncome { get; set; }
        public decimal PaidOutflow { get; set; }
        public decimal CommittedOutflow { get; set; }
        public decimal FreeToSpend { get; set; }
        public decimal FreeToInvest { get; set; }
        public string? Notes { get; set; }
        public DateTime ClosedAt { get; set; }

        public long HouseholdId { get; set; }
        public Household Household { get; set; } = default!;
    }
}
