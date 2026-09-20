using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("MoonAndMilkDb") ?? throw new InvalidOperationException("Connection string 'MoonAndMilkDb' not found.");
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});
builder.Services.AddSingleton<TokenService>();

string jwtSigningKey = builder.Configuration.GetValue<string>("Jwt:SigningKey") ?? throw new InvalidOperationException("Jwt:SigningKey not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapBabiesEndpoints();
app.MapFeedLogsEndpoints();
app.MapDiaperLogsEndpoints();
app.MapSleepLogsEndpoints();
app.MapPumpLogsEndpoints();

app.Run();
