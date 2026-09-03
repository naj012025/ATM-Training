namespace AtmApi.Models;

public sealed class BankAccount
{
    public int Id { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public string PinHash { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public int FailedPinAttempts { get; private set; }
    public bool IsLocked { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    //The = [] means Empty Collection of whatever collection before the setters.
    //under means Every BankAccount Starts with a empty list.
    public List<BankTransaction> Transaction { get; private set; } = [];

    private BankAccount()
    {
        // Empty atm.
    }

    public BankAccount(string accountNumber, string pinHash, decimal openingBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentOutOfRangeException("Account Number is Required", nameof(accountNumber));

        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance));

        AccountNumber = accountNumber;
        PinHash = pinHash;
        Balance = openingBalance;
    }

    public void Deposit(decimal amount)
    {
        //checks for 0 or less than.
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
    }

    public void WithDraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
        if (amount > Balance)
            throw new ArgumentOutOfRangeException("No money bro!");
        Balance -= amount;
    }

    //Counts Failed attemtps and looks account if more than 3 failed attempts.
    public void RegisterFailedAttempts()
    {
        FailedPinAttempts++;

        if (FailedPinAttempts >= 3)
            IsLocked = true;
    }

    //resets the counter when succsefull loggin.
    public void ResetFailsPinAttempts()
    {
        FailedPinAttempts = 0;
    }

}
