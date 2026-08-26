using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly string _signingKey;

    public TokenService(IConfiguration configuration)
    {
        _signingKey = configuration.GetValue<string>("Jwt:SigningKey") ?? throw new InvalidOperationException("Jwt:SigningKey not configured.");
    }

    public string GenerateToken(User user)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_signingKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}