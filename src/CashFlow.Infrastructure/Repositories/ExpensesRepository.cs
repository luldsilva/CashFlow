using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class ExpensesRepository : IExpensesReadOnlyRepository, IExpensesWriteOnlyrepository, IExpensesUpdateOnlyrepository
    {
        private readonly CashFlowDbContext _dbContext;
        public ExpensesRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(Expense expense)
        {           
            await _dbContext.Expenses.AddAsync(expense);
        }

        public async Task<bool> Delete(long id)
        {
            var result = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id);

            if(result is null)
            {
                return false;
            }

            _dbContext.Expenses.Remove(result);

            return true;
        }

        public async Task<List<Expense>> GetAll()
        {
            //Considerando que nao tem id de usuario para filtrar 
            //AsNoTracking evita do enity framework guardar em cache as informcacoes vinda da base, melhora performance da query e gasta menos memoria
            //So deve ser usado caso tenhamos certeza de que quem usa o getAll nao deve alterar os seus dados.
            return await _dbContext.Expenses.AsNoTracking().ToListAsync();
        }

        async Task<Expense?> IExpensesReadOnlyRepository.GetById(long id)
        {
            return await _dbContext.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        async Task<Expense?> IExpensesUpdateOnlyrepository.GetById(long id)
        {
            return await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        }

        public void Update(Expense expense)
        {
            _dbContext.Expenses.Update(expense);
        }
    }
}
