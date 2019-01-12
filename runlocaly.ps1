
# SETUP
# DB Recovery: drop database if exists xgen; RESTORE DATABASE xgen FROM DISK =  N'C:\db-backup\xgen.bak'  WITH MOVE 'xgen' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.mdf', MOVE 'xgen_Log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.ldf'
# RUN:
# 1. Run 2 mock serivices  (SQL-FILE and FILE-FILE)
# 2. Insert a new Deal to DealQueue table - i.e.:  insert into [dbo].[DealQueue] ([Shareclass], [Amount], [TradeDate]) VALUES('AAA', 100, GETDATE())
# 3. Mockservice (SQL-FILE) will move DEAL to Deal table and clean the DealQueue table
# 4. SQLData.xml file with list of all deals will be created in IN folder
# 5. The file will be moved to archive folder.
# 6. MockSerivice (FILE-FILE) will pick up the file and move to OUT folder.


$root=(Resolve-Path .).Path

dotnet mock-service-app\MockService\bin\Debug\netcoreapp2.1\MockService.dll `
	--mockServiceMode  file-file `
	--inFolder $root"\data\in" `
	--outFolder $root"\data\out" 
	
	
$root=(Resolve-Path .).Path
	
dotnet mock-service-app\MockService\bin\Debug\netcoreapp2.1\MockService.dll `
	--mockServiceMode  sql-file `
	--inFolder $root"\data\in" `
	--outFolder $root"\data\out" `
	--connectionString "Data Source=127.0.0.1;Initial Catalog=xgen;User ID=sa;Password=Password1" `
	--insertSqlCommand "INSERT INTO dbo.Deal Select Shareclass, Amount, TradeDate from  dbo.DealQueue; Delete from dbo.DealQueue;" `
	--selectSqlCommand "SELECT Shareclass, Amount, TradeDate FROM dbo.Deal;"
	 
	
	