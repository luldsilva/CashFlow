using AutoMapper;
using CashFlow.Application.UseCases.ExpenseCategories.Resolve;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Repositories.Households;
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
        private readonly IHouseholdReadOnlyRepository _householdRepository;

        public UpdateExpenseUseCase(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IExpensesUpdateOnlyrepository repository,
            ILoggedUser loggedUser,
            IHouseholdReadOnlyRepository householdRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _loggedUser = loggedUser;
            _householdRepository = householdRepository;
        }

        public async Task Execute(long id, RequestExpense request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            var expense = await _repository.GetById(loggedUser, id);

            if (expense is null)
            {
                throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
            }

            _mapper.Map(request, expense);

            var category = ExpenseCategoryResolver.ResolveOptional(household, request.CategoryId, request.CategoryName);
            expense.ExpenseCategoryId = category?.Id;
            expense.ExpenseCategory = category;

            _repository.Update(expense);
            await _unitOfWork.Commit();
        }

        private static void Validate(RequestExpense request)
        {
            var validator = new ExpenseValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
