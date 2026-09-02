Had problems with git Reminder to self do it correctly at the start makes life easyer.

Change in future would be to make this into a api and a database in postgress
.


###TempCode to seed Data###
//PasswordHasher hasher = new();
//AccountRepo repository = new();

//List<BankAccountData> accounts =
//    [
//        new()
//        {
//            AccountNumber= "10001",
//            PinHash = hasher.HashPin("1234"),
//            Balance = 500m
//        },
//        new()
//        {
//            AccountNumber = "10002",
//            PinHash = hasher.HashPin("4567"),
//            Balance = 2000m
//        }
//     ];
//repository.SavedAccounts(accounts);

#########################################################

13. Improvement Tips - Next Iteration
Once the core simulator is stable, improve it in this order rather than adding features randomly.
Priority Improvement Why it matters
1 xUnit tests for Deposit/Withdraw Protects the financial rules from
regressions. Test zero, negative,
exact balance, and insufficient
funds.
2 Separate AuthenticationService Moves account/PIN verification out
of Program.cs and keeps the UI
thin.
3 Async file I/O Use
ReadAllTextAsync/WriteAllTextAsy
nc to practice modern I/O patterns;
useful if persistence grows.
4 Transaction history Add Transaction records with type,
amount, UTC timestamp, and
resulting balance.
5 Atomic/safer persistence Write to a temporary file and
replace the original to reduce
corruption risk if a save is
interrupted.
6 Culture-aware money
parsing/formatting
Control NOK display and decimal
separators explicitly rather than
depending blindly on machine
culture.
7 Lockout state Persist failed attempts / lockeduntil time if you want login
protection to survive app restarts.
8 Dependency Injection After services are separated, inject
repository/hasher dependencies
instead of constructing them
everywhere.
9 Database upgrade Replace JSON with
SQLite/PostgreSQL only after the
domain and repository boundary
are stable.
10 API version Expose the same domain through
ASP.NET Core later. The
BankAccount rules should survive
without being rewritten.