using Amazon.Runtime;
using Amazon.S3;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Domain.Repositories.PasswordResetTokens;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Security.Tokens;
using CashFlow.Domain.Services.Email;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Domain.Services.Storage;
using CashFlow.Infrastructure.DataAccess;
using CashFlow.Infrastructure.Email;
using CashFlow.Infrastructure.Extensions;
using CashFlow.Infrastructure.PasswordReset;
using CashFlow.Infrastructure.Repositories;
using CashFlow.Infrastructure.Security.Tokens;
using CashFlow.Infrastructure.Storage;
using CashFlow.Infrastructure.Services.LoggedUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.IsTestEnvironment())
            {
                AddDbContext(services, configuration);
            }

            AddToken(services, configuration);
            AddPasswordReset(services, configuration);
            AddStorage(services, configuration);
            AddRepositories(services);

            services.AddScoped < IPasswordEncripter, Security.Cryptography.BCrypt> ();
            services.AddScoped < ILoggedUser, LoggedUser> ();
        }

        private static void AddToken(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpiresMinutes");
            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");

            services.AddScoped<IAccessTokenGenerator>(config => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICreditCardsReadOnlyRepository, CreditCardsRepository>();
            services.AddScoped<ICreditCardsWriteOnlyRepository, CreditCardsRepository>();
            services.AddScoped<ICreditCardStatementsReadOnlyRepository, CreditCardStatementsRepository>();
            services.AddScoped<ICreditCardStatementsWriteOnlyRepository, CreditCardStatementsRepository>();
            services.AddScoped<IExpensesReadOnlyRepository, ExpensesRepository>();
            services.AddScoped<IExpensesWriteOnlyrepository, ExpensesRepository>();
            services.AddScoped<IExpensesUpdateOnlyrepository, ExpensesRepository>();
            services.AddScoped<IFinancialObligationsReadOnlyRepository, FinancialObligationsRepository>();
            services.AddScoped<IFinancialObligationsWriteOnlyRepository, FinancialObligationsRepository>();
            services.AddScoped<IFinancialObligationsUpdateOnlyRepository, FinancialObligationsRepository>();
            services.AddScoped<IHouseholdReadOnlyRepository, HouseholdRepository>();
            services.AddScoped<IHouseholdWriteOnlyRepository, HouseholdRepository>();
            services.AddScoped<IMonthlyClosuresReadOnlyRepository, MonthlyClosuresRepository>();
            services.AddScoped<IMonthlyClosuresWriteOnlyRepository, MonthlyClosuresRepository>();
            services.AddScoped<IMonthlyClosuresUpdateOnlyRepository, MonthlyClosuresRepository>();
            services.AddScoped<IPasswordResetTokensReadOnlyRepository, PasswordResetTokensRepository>();
            services.AddScoped<IPasswordResetTokensWriteOnlyRepository, PasswordResetTokensRepository>();
            services.AddScoped<IPasswordResetTokensUpdateOnlyRepository, PasswordResetTokensRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserRepository>();
            services.AddScoped<IUserUpdateOnlyRepository, UserRepository>();
            services.AddScoped<IUserWriteOnlyRepository, UserRepository>();
        }

        private static void AddPasswordReset(IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection("PasswordReset").Get<PasswordResetSettings>() ?? new PasswordResetSettings();

            services.AddSingleton(settings);
            services.AddSingleton<PasswordResetEmailDebugStore>();
            services.AddScoped<IEmailSender, DevelopmentEmailSender>();
        }

        private static void AddStorage(IServiceCollection services, IConfiguration configuration)
        {
            var storageSettings = configuration.GetStorageSettings();

            services.AddSingleton(storageSettings);
            services.AddSingleton<IAmazonS3>(_ =>
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = storageSettings.Endpoint,
                    ForcePathStyle = true
                };

                var credentials = new BasicAWSCredentials(
                    storageSettings.AccessKey,
                    storageSettings.SecretKey);

                return new AmazonS3Client(credentials, config);
            });
            services.AddScoped<IFileStorageService, S3CompatibleFileStorageService>();
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection");
            var configuredServerVersion = configuration.GetValue<string>("Database:ServerVersion");
            var serverVersion = string.IsNullOrWhiteSpace(configuredServerVersion)
                ? ServerVersion.AutoDetect(connectionString)
                : ServerVersion.Parse(configuredServerVersion);

            services.AddDbContext<CashFlowDbContext>(config => config.UseMySql(connectionString, serverVersion));
        }
    }
}
