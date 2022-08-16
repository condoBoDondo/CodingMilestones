# Car Dealership
Final assessment project done for The Software Guild.

Involved full-stack development to build a front-end website, Web API, and database for a car dealership.

Contains Visual Studio solution and projects, as well as a DB folder for SQL database setup and stored procedures.
SQL files are meant to be run in SQL Server Management Studio.

After pulling repository, run contents in this order:
1. run database.sql
2. migrate the CarDealership project via NuGet package manager
3. run tables.sql
4. run reset.sql
5. execute DbReset (the sproc created by reset.sql)
6. run sprocs.sql
7. run CarDealership solution through CarDealership.UI project
