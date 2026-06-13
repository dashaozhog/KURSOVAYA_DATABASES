KURSOVAYA_DATABASES — How to provide / load the database

Overview

This WinForms project uses PostgreSQL via Npgsql. The app expects a PostgreSQL server and a database containing an employees table (used by AuthService). Program.cs currently constructs DataBaseManagement with a connection string — adjust it to point at your local server.

Prerequisites

- .NET 9 SDK or Visual Studio that targets net9.0-windows
- PostgreSQL (server and psql client)
- (Already referenced) Npgsql NuGet package (PackageReference is in the .csproj)

Quick steps (Windows)

1) Install PostgreSQL
   - Download and install from https://www.postgresql.org/download/windows/ or use the installer (includes pgAdmin and psql).

2) Create database and role (example using psql):
   Open "SQL Shell (psql)" or a terminal and run:

   -- login as postgres superuser (you will be prompted for password)
   psql -U postgres -h localhost

   -- create a dedicated database and user (change password):
   CREATE DATABASE kursovaya_db;
   CREATE USER kursovaya_user WITH PASSWORD 'secure_password_here';
   GRANT ALL PRIVILEGES ON DATABASE kursovaya_db TO kursovaya_user;

3) Create required table(s)

   Connect to the new database and run the SQL below (psql or pgAdmin SQL tool):

   \c kursovaya_db

   CREATE TABLE public.employees (
     id SERIAL PRIMARY KEY,
     login TEXT UNIQUE NOT NULL,
     user_password TEXT NOT NULL,
     full_name TEXT
   );

   -- optional: insert a test user
   INSERT INTO public.employees (login, user_password, full_name) VALUES ('admin','admin','Administrator');

4) Configure connection string

   Program.cs currently contains an example connection string near the start of Main():
     var dbService = new DataBaseManagement("Host=localhost;Database=postgres;Username=postgres;Password=..." );

   Edit that string to point to the database created above, for example:
     "Host=localhost;Database=kursovaya_db;Username=kursovaya_user;Password=secure_password_here"

   Note: For better security, consider storing the connection string outside source code (environment variable, config file) before committing. This project currently embeds it in Program.cs.

5) Restore, build, run

   - From command line in project root:
       dotnet restore
       dotnet build
       dotnet run

   - Or open the solution in Visual Studio and run the project.

6) Troubleshooting

   - "Connection failed" message: check that PostgreSQL service is running, connection string credentials are correct, firewall allows local connections, and the database exists.
   - If Npgsql missing, run: dotnet add package Npgsql --version 10.0.3

Security note

Do not commit real passwords into source control. Replace embedded credentials with environment variables or user secrets for production.

If you want, provide a SQL dump file (e.g., schema.sql) in the repo and instructions here; the README can be updated to show how to import it using psql or pg_restore.

If any step should be more specific to your environment, state your OS and PostgreSQL version and an updated instruction can be added.