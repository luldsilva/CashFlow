using CashFlow.Communication.Requests;

namespace CashFlow.Application.UseCases.Users.ResetPassword
{
    public interface IResetPasswordUseCase
    {
        Task Execute(RequestResetPassword request);
    }
}
