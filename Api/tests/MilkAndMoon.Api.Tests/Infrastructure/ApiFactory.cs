using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MilkAndMoon.Api.Data;
using Testcontainers.PostgreSql;

namespace MilkAndMoon.Api.Tests.Infrastructure;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:17").Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:MoonAndMilkDb", _database.GetConnectionString());
        builder.UseSetting("Jwt:SigningKey", "test-signing-key-that-is-at-least-32-bytes-long");
    }

    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync();

        using IServiceScope scope = Services.CreateScope();
        AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _database.DisposeAsync();
    }
}
