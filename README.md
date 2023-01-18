## Useful Commands

### Creating Migration
cd Studio-backend
dotnet ef migrations add InitialCreate -s API -p Persistence

### Dropping the database
cd Studio-backend
dotnet ef database drop

### Dropping the latest migration
Delete the whole migrations folder from the Persistence directory
