using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;

namespace CashFlow.Application.UseCases.Expenses.GetAll
{
    public class GetAllExpenseUseCase : IGetAllExpensesUseCase
    {
        private readonly IExpensesReadOnlyRepository _reporsitory;
        private readonly IMapper _mapper;
        private readonly ILoggedUser _loggedUser;

        public GetAllExpenseUseCase(IExpensesReadOnlyRepository repository, IMapper mapper, ILoggedUser loggedUser)
        {
            _reporsitory = repository;
            _mapper = mapper;
            _loggedUser = loggedUser;
        }
        public async Task<ResponseExpenses> Execute()
        {
            var loggedUser = await _loggedUser.Get();

            var result = await _reporsitory.GetAll(loggedUser);

            return new ResponseExpenses
            {
                Expenses = _mapper.Map<List<ResponseShortExpense>>(result)
            };
        }
    }
}
