Add-Migration InitialCreate -OutputDir Data\Migration
update-database
dotnet ef migrations add InitialCreate -o Data/Migration
dotnet ef database update