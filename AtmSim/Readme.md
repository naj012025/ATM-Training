This Project simulates a Atm, Deposits/Withdraw/TransactionHistory/authentication/Pin/pinhashing.

During this project i learned alot of new syntax and errors that needed to be fixed alon the way 

I turned this into a api version also wich was super informative and also in the end i connected it 
to a another api made before todolistapirecreate1 to learn how to connect api togheter 
atmsim is the main provider of token that is gonna be accepted into todolistapi,

Hadd alot of issuses making the program.cs be correct in apirecreatet it used to use swagger 
but changed it over to scalar so its the same as atm i used chatgpt in the end to clean it up in program.cs

Had issues authorizing in todo list and it was becuse when i entered the qutoes on the token it became invalid 
use alot of time trying to figure this small mistake out but learned alot along the way about jwt loggin and troubleshooting

it is complete for the moment but i plan to combine the Database so i can see both todo and bankaccount info at the same time.

Learne a horrible mistake can be done with docker compose down at that was -v deletes ur db i had atmapi with lots of different
account seeded so i needed to recreate the first seed .

And finaly in todolistapirecreate1 horrible name i know. but when switching from swagger to scaler
after unninstalling the packages i need to doublecheck in xml that ref is gone for swagger info.

and clean/restore/build after. that solved loads of jwtbearer using issuse ghost refrence.

