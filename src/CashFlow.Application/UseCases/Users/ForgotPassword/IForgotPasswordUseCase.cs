using CashFlow.Communication.Requests;

namespace CashFlow.Application.UseCases.Users.ForgotPassword
{
    public interface IForgotPasswordUseCase
    {
        Task Execute(RequestForgotPassword request);
    }
}
