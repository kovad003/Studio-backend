## Useful Commands

### Creating Migration
* cd Studio-backend
* dotnet ef migrations add InitialCreate -s API -p Persistence

### Dropping the database
* cd Studio-backend
* dotnet ef database drop

### Dropping the latest migration
* cd Studio-backend
* dotnet ef migrations remove --force -p Persistence -s API

### Removing unnecessary files from GH tracing
<p>For these items gitignore is not always working so you may need to remove them manually from tracing.</p>

* git rm -r API/obj
* git rm -r API/bin
* git rm -r Application/obj
* git rm -r Application/bin
* git rm -r Domain/bin
* git rm -r Domain/bin
* git rm -r Persistence/bin
* git rm -r Persistence/bin