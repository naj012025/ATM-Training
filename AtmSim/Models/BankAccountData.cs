using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace AtmSim.Models;

internal class BankAccountData
{
    public string AccountNumber { get; set; } = string.Empty;
    public string PinHash { get; set; } = string.Empty;
    public decimal Balance { get; set; }

    public void Deposit(decimal amount)
    {

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount),
                "Deposit must be greater than 0.!");
        }
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > Balance)
        {
            throw new InvalidOperationException(
                "Cannot withdraw more than Account Balance!.");
        }
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount),
                "Amount must be more than: 0!.");
        }

        Balance -= amount;
    }

    public bool VerifyPin(string enteredPin)
    {
        return PinHash == enteredPin;
    }


}
