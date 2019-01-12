# ABV-Invest
That's my final project for C# Web Developer -> a Stock Exchange Broker Application for client usage

The application is designed to provide an easy and secure way for clients to access the information about their investments on the stock exchange. Each client can register on the website with username and PIN provided by their broker plus personal password. 

A non-logged user can access information about the company including contacts and be able to read the latest financial news from Bulgarian Stock Exchange, www.investor.bg and www.capital.bg . Please note that the news page is being reloaded every 30 sec and you can open each link in a new tab.

All logged clients are able to see their portfolio meaning all securities owned by them at the end of each working day. They can also access all information about the deals made on their behalf on each working day, as well as information about their balance – how much money they have in cash and in securities according to the market prices on the day.
Username: 0000000001, PIN: 00001, Pass: 789-Asd

A logged administrator can upload portfolios and deals for each day separately. This is made so to ensure the information uploaded is correct and final. The administrator can upload XML files with specific elements included. If any information is not saved in the database, you will get a message, stating the reason. Apart from this the administrator can add currency types, market types and types of securities. 
Username: ADMIN, PIN: 00000, Pass: 789-Admin

All logged users have an access to their profile, where they can change their password, e-mail and phone number, can confirm their e-mail, download all their personal information and also delete their account according the GDPR requirements.

https://abvinvest.azurewebsites.net 