using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Users.ChangePassword
{
    public class ChangePasswordUseCase : IChangePasswordUseCase
    {
        private readonly IUserUpdateOnlyRepository _repository;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordUseCase(
            IUserUpdateOnlyRepository repository,
            IPasswordEncripter passwordEncripter,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _passwordEncripter = passwordEncripter;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(RequestChangePassword request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var user = await _repository.GetById(loggedUser.Id);

            if (user is null)
            {
                throw new NotFoundException("User was not found.");
            }

            if (!_passwordEncripter.Verify(request.CurrentPassword, user.Password))
            {
                throw new ErrorOnValidationException(["Current password is invalid."]);
            }

            user.Password = _passwordEncripter.Encrypt(request.NewPassword);

            _repository.Update(user);
            await _unitOfWork.Commit();
        }

        private static void Validate(RequestChangePassword request)
        {
            var validator = new ChangePasswordValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }
    }
}
