using AtmApi.Data;
using AtmApi.DTO;
using AtmApi.Models;
using Microsoft.EntityFrameworkCore;


namespace AtmApi.Services;

public sealed class AccountService(AppDbContext db)
{
    public async Task<AccountResponse?> GetAsync(int accountId)
    {
        return await db.Accounts.AsNoTracking()
            .Where(x => x.Id == accountId)
            .Select(x => new AccountResponse
            {
                AccountNumber = x.AccountNumber,
                Balance = x.Balance
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AccountResponse?> DepositAsync(int accountId, decimal amount)
    {
        BankAccount? account = await db.Accounts.FindAsync(accountId);
        if (account is null)
            return null;

        account.Deposit(amount);

        db.Transactions.Add(new BankTransaction(
            account.Id,
            TransactionType.Deposit,
            amount,
            account.Balance));

        await db.SaveChangesAsync();
        return Map(account);

    }

    public async Task<AccountResponse> WithdrawAsync(int accoundId, decimal amount)
    {
        BankAccount? account = await db.Accounts.FindAsync(accoundId);
        if (account is null)
            return null;

        account.WithDraw(amount);

        db.Transactions.Add(new BankTransaction(
            account.Id,
            TransactionType.Withdrawal,
            amount,
            account.Balance));

        await db.SaveChangesAsync();
        return Map(account);
    }

    public async Task<List<TransactionResponse>> GetTransactionsAsync(int accountId)
    {
        return await db.Transactions
            .AsNoTracking()
            .Where(x => x.BankAccountId == accountId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new TransactionResponse
            {
                Id = x.Id,
                Type = x.Type.ToString(),
                Amount = x.Amount,
                BalanceAfter = x.BalanceAfter,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync();
    }
    private static AccountResponse Map(BankAccount account) => new()
    {
        AccountNumber = account.AccountNumber,
        Balance = account.Balance
    };
}
