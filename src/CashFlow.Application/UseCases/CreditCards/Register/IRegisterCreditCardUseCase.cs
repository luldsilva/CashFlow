using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.CreditCards.Register
{
    public interface IRegisterCreditCardUseCase
    {
        Task<ResponseCreditCard> Execute(RequestCreditCard request);
    }
}
