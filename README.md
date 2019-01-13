# legacytesting

POC for legacy testing. The idea is to play a bit to see how docker may help to run/test legacy windows applications. 

## How to use?

1. Recreate database sample from backup.
```sql
drop database if exists xgen; 
RESTORE DATABASE xgen FROM DISK =  N'<path>\xgen.bak'  WITH MOVE 'xgen' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.mdf', MOVE 'xgen_Log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.ldf'
```
2. Run 2 mock services  (SQL-FILE and FILE-FILE)
3. Insert a new Deal to DealQueue table
```sql
insert into [dbo].[DealQueue] ([Shareclass], [Amount], [TradeDate]) VALUES('AAA', 100, GETDATE())
```
4. Mockservice (SQL-FILE) will move DEAL to Deal table and clean the DealQueue table
5. SQLData.xml file with list of all deals will be created in IN folder
6. The file will be moved to archive folder.
7. MockSerivice (FILE-FILE) will pick up the file and move to OUT folder.

## Run locally

### Run MockSerivce (File-File)
```powershell
$root=(Resolve-Path .).Path

dotnet mock-service-app\MockService\bin\Debug\netcoreapp2.1\MockService.dll `
	--mockServiceMode  file-file `
	--inFolder $root"\data\in" `
	--outFolder $root"\data\out" 
```

### Run MockSerivce (SQL-File)
```powershell
$root=(Resolve-Path .).Path
	
dotnet mock-service-app\MockService\bin\Debug\netcoreapp2.1\MockService.dll `
	--mockServiceMode  sql-file `
	--inFolder $root"\data\in" `
	--outFolder $root"\data\out" `
	--connectionString "Data Source=127.0.0.1;Initial Catalog=xgen;User ID=sa;Password=Password1" `
	--insertSqlCommand "INSERT INTO dbo.Deal Select Shareclass, Amount, TradeDate from  dbo.DealQueue; Delete from dbo.DealQueue;" `
	--selectSqlCommand "SELECT Shareclass, Amount, TradeDate FROM dbo.Deal;"
```
   
## Run with docker
 
Run
```powershell

# Set root folder 
$root=(Resolve-Path .).Path

# Copy files to a Volume to make them available later on by containers
docker volume create  dockerfiles_shared
docker run --name copy-helper --rm -v dockerfiles_shared:C:\shared -v $root\:c:\shared_host  microsoft/nanoserver powershell Copy-Item c:\shared_host\* -Destination C:\shared -Recurse -Force

# Run containers (2 services + 1 SQLServer)
docker-compose up --build  --detach

# Recover DB
docker exec -it dockerfiles_mssql_1 SQLCMD  -S "127.0.0.1" -U SA -P Password1 -Q "drop database if exists xgen; RESTORE DATABASE xgen FROM DISK =  N'C:\shared\db-backup\xgen.bak'  WITH MOVE 'xgen' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.mdf', MOVE 'xgen_Log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.ldf'"
```

