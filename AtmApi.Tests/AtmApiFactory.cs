using AtmApi.Data;
using AtmApi.Models;
using AtmApi.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace AtmApi.Tests;

public sealed class AtmApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        // using underscore instead of context since context is never used.
        // Underscore can be used to say i dont care about this but is needed for the compiler.
        builder.ConfigureAppConfiguration((_, config) =>
        {
            Dictionary<string, string?> settings = new()
            {
                ["Jwt:Issuer"] = "AtmApi.Tests",
                ["Jwt:Audience"] = "AtmApi.Tests",
                ["Jwt:Key"] = "ATM-API-TEST-ONLY-SIGNING-KEY-1234567890"

            };
            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            var dbConfig = services.FirstOrDefault(
                d => d.ServiceType ==
                typeof(IDbContextOptionsConfiguration<AppDbContext>));
            if (dbConfig is not null)
                services.Remove(dbConfig);

            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("Datasource=:memory:");

                connection.Open();
                return connection;
            });

            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                DbConnection connection = provider.GetRequiredService<DbConnection>();

                options.UseSqlite(connection);
            });
        });

    }
    public async Task SeedAccountAsync(
    string accountNumber,
    string pin,
    decimal openingBalance)
    {
        using IServiceScope scope = Services.CreateScope();

        AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        PinHasher hasher = scope.ServiceProvider.GetRequiredService<PinHasher>();

        await db.Database.EnsureCreatedAsync();

        db.Accounts.Add(
            new BankAccount(
                accountNumber,
                hasher.HashPin(pin),
                openingBalance));

        await db.SaveChangesAsync();
    }


}
