using AtmApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AtmApi.Security;
//Learned that using the microsoft.identitymodel.jasonwebtokens
//is the never way in dotnet but stikking with this for now.
public sealed class TokenService(IConfiguration configuration)
{
    public string CreateTokens(BankAccount account)
    {

        string key = configuration["Jwt:Key"]!;
        string issuer = configuration["Jwt:Issuer"]!;
        string audience = configuration["Jwt:audience"]!;

        Claim[] claims =
        {
        new(ClaimTypes.NameIdentifier,account.Id.ToString()),
        new("account_number", account.AccountNumber)
    };
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));

        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: issuer, //program
            audience: audience,//inteed for user
            claims: claims,//user info.
            expires: DateTime.UtcNow.AddMinutes(15),//les time to use it more secure but less time.
            signingCredentials: credentials);//Protection against token change.

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
