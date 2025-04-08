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

            if (result is null)
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

        async Task<Expense?> IExpensesUpdateOnlyrepository.GetById(User user, long id)
        {
            return await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);
        }

        public void Update(Expense expense)
        {
            _dbContext.Expenses.Update(expense);
        }

        public async Task<List<Expense>> FilterByMonth(DateOnly date)
        {
            var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date;

            var daysInMonth = DateTime.DaysInMonth(year: date.Year, month: date.Month);

            var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth, hour: 23, minute: 59, second: 59);

            return await _dbContext.Expenses
                .AsNoTracking()
                .Where(e => e.Date > startDate && e.Date <= endDate)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Title)
                .ToListAsync();
        }
    }
}
