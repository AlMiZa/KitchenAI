using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KitchenAI.Application.Services;
using KitchenAI.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KitchenAI.Infrastructure.Services;

/// <summary>Generates signed HS256 JWT tokens containing user identity and household scope.</summary>
public class TokenService(IConfiguration configuration) : ITokenService
{
    /// <inheritdoc/>
    public string GenerateToken(User user, Guid householdId)
    {
        ArgumentNullException.ThrowIfNull(user);

        var secretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        var issuer = configuration["Jwt:Issuer"] ?? "KitchenAI";
        var audience = configuration["Jwt:Audience"] ?? "KitchenAI";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("householdId", householdId.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
