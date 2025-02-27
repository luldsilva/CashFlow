using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Infrastructure.DataAccess;
using CashFlow.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddRepositories(services);

            services.AddScoped < IPasswordEncripter, Infrastructure.Security.BCrypt> ();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IExpensesReadOnlyRepository, ExpensesRepository>();
            services.AddScoped<IExpensesWriteOnlyrepository, ExpensesRepository>();
            services.AddScoped<IExpensesUpdateOnlyrepository, ExpensesRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        }
        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection"); ;
            var serverVersion = new MySqlServerVersion(new Version());

            services.AddDbContext<CashFlowDbContext>(config => config.UseMySql(connectionString, serverVersion));
        }
    }
}
