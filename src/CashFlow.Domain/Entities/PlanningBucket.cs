namespace CashFlow.Domain.Entities
{
    public class PlanningBucket
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }

        public long HouseholdId { get; set; }
        public Household Household { get; set; } = default!;
    }
}
