using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Expenses;

namespace CashFlow.Application.UseCases.Expenses.GetAll
{
    public class GetAllExpenseUseCase : IGetAllExpensesUseCase
    {
        private readonly IExepensesRepository _reporsitory;
        private readonly IMapper _mapper;

        public GetAllExpenseUseCase(IExepensesRepository repository, IMapper mapper)
        {
            _reporsitory = repository;
            _mapper = mapper;
        }
        public async Task<ResponseExpenses> Execute()
        {
            var result = await _reporsitory.GetAll();

            return new ResponseExpenses
            {
                Expenses = _mapper.Map<List<ResponseShortExpense>>(result)
            };
        }
    }
}
