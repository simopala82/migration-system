Ecco il contenuto Markdown completo e unificato del documento, pronto per essere copiato e incollato direttamente in un file, mantenendo tutta la formattazione (codice, tabelle, link):# 1. README: Piattaforma Asincrona di Migrazione Utente (OLD -> NEW)

## 🎯 Project Overview

This solution implements a controlled, asynchronous platform for migrating user data from a Legacy system (`OLD`) to a new platform (`NEW`) under constraints of data integrity, limited concurrency, and administrative traceability. The system is designed to ensure data integrity using the **SAGA pattern (compensation/rollback)** and prevent system overload through a **Throttling mechanism (Slot Manager)**.

The architecture is modular and ready for seamless integration with a Message Broker system.

## 📁 Directory Structure

| **Directory** | **Content** | **Notes** | 
| :--- | :--- | :--- | 
| `src/` | Source code for the entire solution (API, Core, DataAccess, Worker). | Contains all .NET Core projects. | 
| `docs/` | Project documentation. | Contains the technical and functional analyses. | 
| `tools/` | Testing and interaction tools. | Contains the Postman Collection. | 

## 🚀 Project Startup

The project utilizes three SQL Server-based databases (OLD, NEW, Audit). To run it in a development environment, you need a SQL Server Docker/Podman container.

### 1. Starting the SQL Server Container

Use the following command to start the SQL Server container and accept the EULA:

```
podman run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P5tNqWwhFaoNHE6APe13" -p 1433:1433 -v sql-server-volume:/data --name sql-server -h sql-server -d mcr.microsoft.com/mssql/server:2019-latest
```
> **NOTE:** The password is set to `P5tNqWwhFaoNHE6APe13`. Ensure the connection strings in the projects are aligned with these values.

### 2. Creating and Seeding Databases

After starting the container, apply the Entity Framework Core migrations to create the schemas for all three databases:

1. Database for the New Platform (NEW)dotnet ef database update --project Migration.DataAccess --startup-project Migration.Worker --context UserNewDbContext2. Legacy Database (OLD) with Seed Datadotnet ef database update --project Migration.DataAccess --startup-project Migration.Worker --context UserOldDbContext3. Audit and Migration Status Databasedotnet ef database update --project Migration.DataAccess --startup-project Migration.Worker --context MigrationDbContext
### 3. Executing the Application

After updating the databases, start the project:

```
dotnet run --project Migration.API
```
You can use postman collection to run the api.

You can execute a single migration worker for a single user:
```
dotnet run --project Migration.Worker -- {id}
```
