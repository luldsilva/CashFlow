using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entities
{
    public class Household
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public bool HasVariableIncome { get; set; }
        public HouseholdIncomeFrequency PrimaryIncomeFrequency { get; set; }
        public PlanningModel PlanningModel { get; set; }

        public long UserId { get; set; }
        public User User { get; set; } = default!;
        public List<CreditCard> CreditCards { get; set; } = [];
        public List<IncomeSource> IncomeSources { get; set; } = [];
        public List<FinancialObligation> FinancialObligations { get; set; } = [];
        public List<MonthlyClosure> MonthlyClosures { get; set; } = [];
        public List<PlanningBucket> PlanningBuckets { get; set; } = [];
        public List<ExpenseCategory> ExpenseCategories { get; set; } = [];
    }
}
