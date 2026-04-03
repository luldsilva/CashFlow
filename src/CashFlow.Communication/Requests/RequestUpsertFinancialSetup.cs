using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Requests
{
    public class RequestUpsertFinancialSetup
    {
        public string HouseholdName { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public bool HasVariableIncome { get; set; }
        public HouseholdIncomeFrequency PrimaryIncomeFrequency { get; set; }
        public PlanningModel PlanningModel { get; set; }
        public List<RequestIncomeSource> IncomeSources { get; set; } = [];
        public List<RequestPlanningBucket> PlanningBuckets { get; set; } = [];
        public List<RequestExpenseCategory> ExpenseCategories { get; set; } = [];
    }
}
