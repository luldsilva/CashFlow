using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.UseCases.CreditCardStatements
{
    internal static class CreditCardStatementResponseMapper
    {
        public static ResponseCreditCardStatement Map(CreditCardStatement statement)
        {
            return new ResponseCreditCardStatement
            {
                Id = statement.Id,
                CreditCardId = statement.CreditCardId,
                CreditCardName = statement.CreditCard.Name,
                CompetenceDate = statement.CompetenceDate,
                ClosingDate = statement.ClosingDate,
                DueDate = statement.DueDate,
                TotalAmount = statement.TotalAmount,
                PaidAmount = statement.PaidAmount,
                PaidDate = statement.PaidDate,
                Status = (CashFlow.Communication.Enums.CreditCardStatementStatus)statement.Status,
                RequiresReview = statement.Status != Domain.Enums.CreditCardStatementStatus.Paid
            };
        }
    }
}
