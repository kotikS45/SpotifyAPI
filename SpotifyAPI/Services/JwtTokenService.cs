using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Model.Entities.Identity;
using SpotifyAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SpotifyAPI.Services;

public class JwtTokenService(
    UserManager<User> userManager,
    IConfiguration configuration
    ) : IJwtTokenService
{

    public async Task<string> CreateTokenAsync(User user)
    {
        var key = Encoding.UTF8.GetBytes(
            configuration["Authentication:Jwt:SecretKey"]
                ?? throw new NullReferenceException("Authentication:Jwt:SecretKey")
        );

        int tokenLifetimeInDays = Convert.ToInt32(
            configuration["Authentication:Jwt:TokenLifetimeInDays"]
                ?? throw new NullReferenceException("Authentication:Jwt:TokenLifetimeInDays")
        );

        var signinKey = new SymmetricSecurityKey(key);

        var signinCredential = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            signingCredentials: signinCredential,
            expires: DateTime.Now.AddDays(tokenLifetimeInDays),
            claims: await GetClaimsAsync(user));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private async Task<List<Claim>> GetClaimsAsync(User user)
    {
        string userEmail = user.Email
            ?? throw new NullReferenceException("User.Email");

        string userName = user.UserName
            ?? throw new NullReferenceException("User.UserName");

        var userRoles = await userManager.GetRolesAsync(user);

        var roleClaims = userRoles
            .Select(r => new Claim(ClaimTypes.Role, r))
            .ToList();

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim("email", userEmail),
            new Claim("image", user.Photo),
            new Claim("username", user.UserName)
        };

        if (!string.IsNullOrEmpty(user.Name))
        {
            claims.Add(new Claim("name", user.Name));
        }
        claims.AddRange(roleClaims);

        return claims;
    }
}