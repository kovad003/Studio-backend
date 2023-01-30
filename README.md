## Requirements
* Download and install .NET 7.0.1
  * Link: https://dotnet.microsoft.com/en-us/download/dotnet/7.0

## How to Run the App
1) Open a Terminal
2) Navigate into the solution folder (Studio-backend)
    * E.g.: cd 'C:\MyRepository\DotNet\Studio-backend'
3) Navigate into the API folder (location of Program.cs)
    * E.g.: cd 'API'
4) Use one of the following command:
    * dotnet run (No INFO displayed)
    * dotnet Watch (INFO displayed)
5) You can stop the application pressing the following key combination:
    * CTRL + C (First click on the terminal window)

## Important Information
* Smaller code changes should be applied automatically by hot reload.
* It is better to restart the app by pressing CTRL + R (First click on the terminal window).

## Attachments
* In the Postman folder you can find Postman collections. Import them into your Postman so you can test the Endpoints of the application.

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