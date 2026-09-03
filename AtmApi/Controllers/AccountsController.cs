using AtmApi.DTO;
using AtmApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AtmApi.Controllers;


[ApiController]
[Authorize]
[Route("api/accounts")]
public sealed class AccountsController(AccountService accountService) : ControllerBase
{
    //The route uses /me instead of accepting any account ID from the caller.
    //The account ID comes from the validated token, reducing accidental cross-account access
    [HttpGet("me")]
    public async Task<ActionResult<AccountResponse>> GetMyAccount()
    {
        int accountId = GetAccountId();
        AccountResponse? account = await accountService.GetAsync(accountId);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost("me/deposits")]
    public async Task<ActionResult<AccountResponse>> Deposit(AmountRequest request)
    {
        try
        {
            AccountResponse? account = await accountService.DepositAsync(
                GetAccountId(), request.Amount);

            return account is null ? NotFound() : Ok(account);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("me/withdrawals")]
    public async Task<ActionResult<AccountResponse>> Withdraw(AmountRequest request)
    {
        try
        {
            AccountResponse? account = await accountService.WithdrawAsync(
                GetAccountId(), request.Amount);

            return account is null ? NotFound() : Ok(account);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("me/transactions")]
    public async Task<ActionResult<List<TransactionResponse>>> GetTransaction()
    {
        return Ok(await accountService.GetTransactionsAsync(GetAccountId()));
    }



    private int GetAccountId()
    {
        string value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();

        return int.Parse(value);
    }
}


