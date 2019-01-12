
# =============== SETUP CLEAN

$root=(Resolve-Path .).Path

docker-compose down
#docker container prune

# Populate dockerfiles_shared volume 
# docker volume prune
# docker volume create  dockerfiles_shared
docker run --name copy-helper --rm -v dockerfiles_shared:C:\shared -v $root\:c:\shared_host  microsoft/nanoserver powershell Copy-Item c:\shared_host\* -Destination C:\shared -Recurse -Force


# Up docker-compose serivces
docker-compose up --build  --detach
#docker-compose up --build  --detach mssql

# Database recovery
Start-Sleep -Seconds 20

docker exec -it dockerfiles_mssql_1 SQLCMD  -S "127.0.0.1" -U SA -P Password1 -Q "drop database if exists xgen; RESTORE DATABASE xgen FROM DISK =  N'C:\shared\db-backup\xgen.bak'  WITH MOVE 'xgen' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.mdf', MOVE 'xgen_Log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\xgen.ldf'"


#docker run -it --rm -w /w -v $root\db-backup\exported:/out -v dockerfiles_shared:/home alpine sh -c "cp -r -f -u /home/db-backup/* /out"
docker run --name copy-helper -it --rm -v dockerfiles_shared:C:\shared -v $root\:c:\shared_host  microsoft/nanoserver powershell Copy-Item c:\shared\db-backup\* -Destination C:\shared_host\db-backup 



# Copy output files to Host drive
docker run -it --rm -w /w -v D:\Wojtek\Azure\volume1:/out -v dockerfiles_shared:/home alpine sh -c "while true; cp -r -f -u /home/data/out/* /out ; do sleep 2 ; done"

# =============== RESTART SETUP

#docker-compose stop

# Populate dockerfiles_shared volume 
#docker run --name copy-helper -it --rm -v dockerfiles_shared:C:\shared -v $root\:c:\shared_host  microsoft/nanoserver powershell Copy-Item c:\shared_host\* -Destination C:\shared -Recurse -Force

docker-compose start

