using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using All4GYM.Models;
using Microsoft.IdentityModel.Tokens;

namespace All4GYM.Services;

public class JwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var hasSub = user.HasActiveSubscription.ToString().ToLowerInvariant();
        var tier = user.SubscriptionTier.ToString();

        Console.WriteLine($"🧪 Генеруємо токен:");
        Console.WriteLine($"→ HasActiveSubscription: {hasSub}");
        Console.WriteLine($"→ SubscriptionTier: {tier}");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
            new Claim("SubscriptionTier", tier),
            new Claim("HasActiveSubscription", hasSub)
        };

        foreach (var claim in claims)
            Console.WriteLine($"✅ Claim → {claim.Type}: {claim.Value}");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}