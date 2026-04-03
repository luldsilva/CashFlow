using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.CreditCardStatements
{
    public interface ICreditCardStatementsWriteOnlyRepository
    {
        Task Add(CreditCardStatement statement);
    }
}
