namespace AtmApi.Models;


//Makes it so Errors are les likely and Type becoms a thing with this contract.
public enum TransactionType
{
    Deposit,
    Withdrawal
}
public sealed class BankTransaction
{
    public int Id { get; set; }
    public int BankAccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private BankTransaction()
    {
        //empty atm.
    }

    public BankTransaction(
        int bankAccountId,
        TransactionType type,
        decimal amount,
        decimal balanceAfter
        )
    {
        BankAccountId = bankAccountId;
        Type = type;
        Amount = amount;
        BalanceAfter = balanceAfter;

    }

}
