using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _repository;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        public DoLoginUseCase(IUserReadOnlyRepository repository, IPasswordEncripter passwordEncripter, IAccessTokenGenerator accessTokenGenerator)
        {
            _repository = repository;
            _passwordEncripter = passwordEncripter;
            _accessTokenGenerator = accessTokenGenerator;
        }

        public async Task<ResponseRegisteredUser> Execute(RequestLogin request)
        {
            var user = await _repository.GetUserByEmail(request.Email);

            if (user is null)
            {
                throw new InvalidLoginException();
            }

            var passwordMatch = _passwordEncripter.Verify(request.Password, user.Password);

            if(passwordMatch ==  false)
            {
                throw new InvalidLoginException();
            }

            return new ResponseRegisteredUser
            {
                Name = user.Name,
                Token = _accessTokenGenerator.Generate(user)
            };
        }
    }
}
