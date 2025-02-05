using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IExepensesRepository, ExpensesRepository>();
        }
    }
}
