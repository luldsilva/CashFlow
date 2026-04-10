using AutoMapper;
using CashFlow.Application.UseCases.ExpenseCategories.Resolve;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Register
{
    public class RegisterExpenseUseCase : IRegisterExpenseUseCase
    {
        private readonly IExpensesWriteOnlyrepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;
        private readonly IHouseholdReadOnlyRepository _householdRepository;

        public RegisterExpenseUseCase(
            IExpensesWriteOnlyrepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILoggedUser loggedUser,
            IHouseholdReadOnlyRepository householdRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _loggedUser = loggedUser;
            _householdRepository = householdRepository;
        }

        public async Task<ResponseRegisteredExpense> Execute(RequestExpense request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            var expense = _mapper.Map<Expense>(request);
            expense.UserId = loggedUser.Id;

            var category = ExpenseCategoryResolver.ResolveOptional(household, request.CategoryId, request.CategoryName);
            expense.ExpenseCategoryId = category?.Id;
            expense.ExpenseCategory = category;

            await _repository.Add(expense);
            await _unitOfWork.Commit();

            return _mapper.Map<ResponseRegisteredExpense>(expense);
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
