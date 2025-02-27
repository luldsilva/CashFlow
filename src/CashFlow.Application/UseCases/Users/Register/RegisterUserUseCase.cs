using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation.Results;

namespace CashFlow.Application.UseCases.Users.Register
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IMapper _mapper;
        private readonly IPasswordEncripter _passwordEncripter;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserUseCase(IMapper mapper, IPasswordEncripter passwordEncripter, IUserReadOnlyRepository userReadOnlyRepository, 
            IUserWriteOnlyRepository userWriteOnlyRepository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _passwordEncripter = passwordEncripter;
            _userReadOnlyRepository = userReadOnlyRepository;
            _userWriteOnlyRepository = userWriteOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseRegisteredUser> Execute(RequestRegisterUser request)
        {
            await Validate(request);

            var user = _mapper.Map<Domain.Entities.User>(request);

            user.Password = _passwordEncripter.Encrypt(request.Password);

            user.UserIdentifier = Guid.NewGuid();

            await _userWriteOnlyRepository.Add(user);

            await _unitOfWork.Commit();

            return new ResponseRegisteredUser
            {
                Name = user.Name
            };

        }

        private async Task Validate(RequestRegisterUser request)
        {
            var result = new RegisterUserValidator().Validate(request);

            var emailAlreadyExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);

            if (emailAlreadyExist)
            {
                result.Errors.Add(new ValidationFailure(string.Empty, ResourceErrorMessages.EMAIL_ALREADY_REGISTERED));
            }

            if(result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(r => r.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
