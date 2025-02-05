using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.Expenses.Register
{
    public class RegisterExpenseUseCase : IRegisterExpenseUseCase
    {
        private readonly IExepensesRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterExpenseUseCase(IExepensesRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public ResponseRegisteredExpense Execute(RequestRegisterExpense request)
        {
            Validate(request);

            var entity = new Expense
            {
                Amount = request.Amount,
                Date = request.Date,
                Description = request.Description,
                Title = request.Title,
                PaymentType = (Domain.Enums.PaymentType)request.PaymentType,
            };

            _repository.Add(entity);

            _unitOfWork.Commit();

            return new ResponseRegisteredExpense();
        }

        private void Validate(RequestRegisterExpense request)
        {
            var validator = new RegisterExpenseValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(f => f.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
