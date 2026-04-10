namespace CashFlow.Domain.Entities
{
    public class ExpenseCategory
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? BucketCode { get; set; }
        public List<Expense> Expenses { get; set; } = [];
        public List<FinancialObligation> FinancialObligations { get; set; } = [];

        public long HouseholdId { get; set; }
        public Household Household { get; set; } = default!;
    }
}
