using System.Security.Cryptography;
using System.Text;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.PasswordResetTokens;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Users.ResetPassword
{
    public class ResetPasswordUseCase : IResetPasswordUseCase
    {
        private readonly IPasswordResetTokensUpdateOnlyRepository _tokensRepository;
        private readonly IUserUpdateOnlyRepository _userRepository;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordUseCase(
            IPasswordResetTokensUpdateOnlyRepository tokensRepository,
            IUserUpdateOnlyRepository userRepository,
            IPasswordEncripter passwordEncripter,
            IUnitOfWork unitOfWork)
        {
            _tokensRepository = tokensRepository;
            _userRepository = userRepository;
            _passwordEncripter = passwordEncripter;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(RequestResetPassword request)
        {
            Validate(request);

            var tokenHash = ComputeSha256(request.Token.Trim());
            var resetToken = await _tokensRepository.GetActiveByTokenHash(tokenHash);

            if (resetToken is null)
            {
                throw new ErrorOnValidationException(["Reset token is invalid or expired."]);
            }

            var user = await _userRepository.GetById(resetToken.UserId);
            if (user is null)
            {
                throw new NotFoundException("User was not found.");
            }

            user.Password = _passwordEncripter.Encrypt(request.NewPassword);
            resetToken.UsedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            _tokensRepository.Update(resetToken);
            await _unitOfWork.Commit();
        }

        private static string ComputeSha256(string value)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(bytes);
        }

        private static void Validate(RequestResetPassword request)
        {
            var validator = new ResetPasswordValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }
    }
}
