using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.CreditCardStatements.GetByMonth
{
    public interface IGetCreditCardStatementsByMonthUseCase
    {
        Task<ResponseCreditCardStatements> Execute(DateTime competenceDate);
    }
}
