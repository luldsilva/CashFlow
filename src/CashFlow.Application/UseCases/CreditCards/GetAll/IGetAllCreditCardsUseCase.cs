using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.CreditCards.GetAll
{
    public interface IGetAllCreditCardsUseCase
    {
        Task<ResponseCreditCards> Execute();
    }
}
