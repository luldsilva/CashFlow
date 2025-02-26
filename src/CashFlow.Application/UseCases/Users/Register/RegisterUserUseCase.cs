using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IMapper _mapper;

        public RegisterUserUseCase(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ResponseRegisteredUser> Execute(RequestRegisterUser request)
        {
            Validate(request);

            var user = _mapper.Map<Domain.Entities.User>(request);

            return new ResponseRegisteredUser
            {
                Name = user.Name
            };

        }

        private void Validate(RequestRegisterUser request)
        {
            var result = new RegisterUserValidator().Validate(request);

            if(result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(r => r.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
