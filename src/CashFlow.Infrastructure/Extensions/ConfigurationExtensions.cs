using CashFlow.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;

namespace CashFlow.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsTestEnvironment(this IConfiguration configuration)
        {
            return configuration.GetValue<bool>("InMemoryTest");
        }

        public static bool ShouldRunDatabaseMigration(this IConfiguration configuration)
        {
            return !configuration.IsTestEnvironment()
                && configuration.GetValue("Database:RunMigrationsOnStartup", true);
        }

        public static StorageSettings GetStorageSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("Storage").Get<StorageSettings>() ?? new StorageSettings();
        }
    }
}
