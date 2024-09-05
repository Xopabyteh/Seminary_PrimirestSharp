# args
$migrationName=$args[0]

# cool text B-)
write-host "
.  . .-. .-. .-. .-. .-. .-.
|\/|  |  |.. |(  |-|  |  |- 
'  ` `-' `-' ' ' ` '  '  `-'
"


# Migrate
dotnet ef migrations add $migrationName --context PrimirestSharpDbContext --project Yearly.Infrastructure --startup-project Yearly.Presentation