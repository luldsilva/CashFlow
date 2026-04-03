using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.CreditCards
{
    public interface ICreditCardsWriteOnlyRepository
    {
        Task Add(CreditCard creditCard);
    }
}
