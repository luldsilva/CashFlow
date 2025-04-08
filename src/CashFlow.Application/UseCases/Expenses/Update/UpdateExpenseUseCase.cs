using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Update
{
    public class UpdateExpenseUseCase : IUpdateExpenseUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExpensesUpdateOnlyrepository _repository;
        private readonly ILoggedUser _loggedUser;

        public UpdateExpenseUseCase(IMapper mapper, IUnitOfWork unitOfWork, IExpensesUpdateOnlyrepository repository, ILoggedUser loggedUser)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _loggedUser = loggedUser;
        }

        public async Task Execute(long id, RequestExpense request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();

            var expense = await _repository.GetById(loggedUser, id);

            if(expense is null)
            {
                throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
            } 

            _mapper.Map(request, expense); //primeiro a fonte dos dados, depois destinos dos dados

            _repository.Update(expense);

            await _unitOfWork.Commit();
        }

        private void Validate(RequestExpense request)
        {
            var validator = new ExpenseValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
