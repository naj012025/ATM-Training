using AtmSim.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtmSim.Models;

public class BankAccount
{
    public string AccountNumber { get; private set; }
    private string Pin { get; }
    public decimal Balance { get; private set; }

    public BankAccount(string accountNumber, string pin, decimal balance)
    {
        AccountNumber = accountNumber;
        Pin = pin;
        Balance = balance;
    }







}
