using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.CreditCardStatements.Register
{
    public interface IRegisterCreditCardStatementUseCase
    {
        Task<ResponseCreditCardStatement> Execute(RequestCreditCardStatement request);
    }
}
