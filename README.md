## Requirements

* Download and install .NET 7.0.1
  * Link: <https://dotnet.microsoft.com/en-us/download/dotnet/7.0>

## How to Run the App

### Development

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

### Production

Running the app with PostgreSQL

#### Prerequisites

1. PostgreSQL: You can install it using native package manager or running with docker
   1. Windows

      ```bash
      winget install PostgreSQL.PostgreSQL
      ```

   2. Mac

      ```bash
      brew install postgresql
      ```

   3. Docker

      ```bash
      docker run --rm -d -p 5432:5432 \
      --name postgres \
      -e POSTGRES_USER=postgres \
      -e POSTGRES_PASSWORD=postgres \
      -e POSTGRES_DB=studio \
      postgres
      ```

#### Steps

1. Open a terminal
2. Navigate into the solution folder (Studio-backend)

   ```bash
   cd Studio-backend
   ```

3. Ensure the database credentials are correct in `./API/appsettings.json`
4. Remove all the previous migration to avoid conflict
   1. ./Persistence/Migrations/*
5. Initialize the PostgreSQL

   ```bash
   ASPNETCORE_ENVIRONMENT=Production dotnet-ef migrations add PostgresInitial -p Persistence -s API

   # this one can be run if we want to trigger the migration manually.
   # But in this case is not needed.
   # ASPNETCORE_ENVIRONMENT=Production dotnet-ef database update -p Persistence -s API
   ```

6. Run the app

   ```bash
   # for deployment on production
   make rerun

   # or run the command below if you want to run the app on Development using PostgreSQL
   ASPNETCORE_ENVIRONMENT=Production dotnet watch --project API
   ```

7. Connect to the PostrgeSQL

   ```bash
   psql -U postgres -W
   ```

8. Connect to the database

   ```postgresql
   \c studio
   ```

9. Commonly used PostgreSQL commands

   ```postgresql
   # list databases
   \l

   # list tables
   \dt

   # list contents of a table
   select * from "<table-name>";
   ```

## Important Information

### Hot Reload

* Smaller code changes should be applied automatically by hot reload.
* It is better to restart the app by pressing CTRL + R (First click on the terminal window).

### Excluded files

* Files containing sensitive information (Secrets, keys) are excluded from the repository because of security reasons.
* You need to set up this file manually on your PC so you can use all the features of the app.
* This applies to the following files:
  * API/appsettings.json

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

```html
<p>For these items gitignore is not always working so you may need to remove them manually from tracing.</p>
```

* git rm -r API/obj
* git rm -r API/bin
* git rm -r Application/obj
* git rm -r Application/bin
* git rm -r Domain/bin
* git rm -r Domain/bin
* git rm -r Persistence/bin
* git rm -r Persistence/bin
