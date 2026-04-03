namespace CashFlow.Application.UseCases.FinancialObligations.Delete
{
    public interface IDeleteFinancialObligationUseCase
    {
        Task Execute(long id);
    }
}
