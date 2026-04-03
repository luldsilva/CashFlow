namespace CashFlow.Domain.Entities
{
    public class ExpenseCategory
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BucketCode { get; set; } = string.Empty;

        public long HouseholdId { get; set; }
        public Household Household { get; set; } = default!;
    }
}
