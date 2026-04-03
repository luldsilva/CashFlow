using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Responses
{
    public class ResponseFinancialSetup
    {
        public long Id { get; set; }
        public string HouseholdName { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public bool HasVariableIncome { get; set; }
        public HouseholdIncomeFrequency PrimaryIncomeFrequency { get; set; }
        public PlanningModel PlanningModel { get; set; }
        public List<ResponseIncomeSource> IncomeSources { get; set; } = [];
        public List<ResponsePlanningBucket> PlanningBuckets { get; set; } = [];
        public List<ResponseExpenseCategory> ExpenseCategories { get; set; } = [];
    }
}
