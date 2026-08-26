using Microsoft.EntityFrameworkCore;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);
    }

    private static async Task<IResult> RegisterAsync(RegisterRequest request, AppDbContext dbContext)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Results.BadRequest("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.BadRequest("Email is required.");
        }

        if (request.Password.Length < 8)
        {
            return Results.BadRequest("Password must be at least 8 characters long.");
        }

        User user = new()
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return Results.Created($"/users/{user.Id}", new UserResponse(user.Id, user.Name, user.Email, user.CreatedAt));
    }

    private static async Task<IResult> LoginAsync(LoginRequest request, AppDbContext dbContext)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Results.BadRequest("Email is required.");
        }

        User? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new UserResponse(user.Id, user.Name, user.Email, user.CreatedAt));
    }
}