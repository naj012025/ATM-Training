using AtmApi.Data;
using AtmApi.DTO.Auth;
using AtmApi.Security;
using Microsoft.EntityFrameworkCore;




namespace AtmApi.Services;

public sealed class AuthService(
    AppDbContext db,
    PinHasher pinHasher,
    TokenService tokenService)
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var account = await db.Accounts.FirstOrDefaultAsync(
            x => x.AccountNumber == request.AccountNumber);

        if (account is null || account.IsLocked)
            return null;

        if (!pinHasher.VerifyPin(request.Pin, account.PinHash))
        {
            account.RegisterFailedAttempts();
            await db.SaveChangesAsync();
            return null;
        }

        account.ResetFailsPinAttempts();
        await db.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = tokenService.CreateTokens(account)
        };
    }
}




