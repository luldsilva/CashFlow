namespace CashFlow.Communication.Responses
{
    public class ResponsePreparedMonth
    {
        public DateTime CompetenceDate { get; set; }
        public DateTime SourceCompetenceDate { get; set; }
        public bool IsMonthClosed { get; set; }
        public List<ResponsePreparedIncomeSource> IncomeSources { get; set; } = [];
        public List<ResponseFinancialObligation> CreatedObligations { get; set; } = [];
        public List<ResponseCreditCardStatement> CreatedCreditCardStatements { get; set; } = [];
        public int SkippedObligations { get; set; }
        public int SkippedCreditCardStatements { get; set; }
    }
}
