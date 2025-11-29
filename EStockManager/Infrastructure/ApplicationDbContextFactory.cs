using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EStockManager.Infrastructure
{
    // EF Core araçları (Add-Migration) bu sınıfı kullanarak DbContext'i oluşturur.
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1. appsettings.json dosyasını okuyabilmek için Configuration oluşturulur.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Bağlantı dizesi okunur.
            var connectionString = configuration.GetConnectionString("PostgreSqlConnection");

            // 3. Options Builder oluşturulur.
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // 4. PostgreSQL sağlayıcısı yapılandırılır.
            builder.UseNpgsql(connectionString);

            // 5. DbContext, doğru Options ile döndürülür.
            return new ApplicationDbContext(builder.Options);
        }
    }
}