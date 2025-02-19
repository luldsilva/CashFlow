namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf
{
    public interface IGenerateExpensesReportPdfuseCase
    {
        Task<byte[]> Execute(DateOnly month);
    }
}
