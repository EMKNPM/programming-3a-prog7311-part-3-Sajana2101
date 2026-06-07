# GLMS Part 3 - Service-Oriented Shipping Management System

This repository contains the final Part 3 implementation of the Global Logistics Management System. The system was refactored from a single MVC application into a service-oriented structure with a separate MVC frontend, Web API backend, SQL Server database container, Docker Compose setup, JWT authentication, and automated API integration tests.

> Important: Please use the `main` branch for the final Part 3 submission.

---

## Project Structure

The solution contains three main projects:

GLMS2               -ASP.NET Core MVC frontend
GLMS2.API2          -ASP.NET Core Web API backend
GLMS2.Test         - Unit and integration tests
docker-compose.yml  -Docker Compose configuration
.dockerignore


## Main Features

The system supports:

* Client management
* Contract management
* PDF upload for signed contract agreements
* Contract status workflow
* Service request management
* USD to ZAR currency conversion using an external API
* JWT-based authentication for protected create, edit, and delete actions
* Swagger/OpenAPI documentation for the backend API
* Docker Compose orchestration for the database, API, and MVC frontend
* Automated tests for API behaviour and workflows

---

## Architecture Overview

The final system is separated into three layers:


MVC Frontend
  
Web API Backend
   
SQL Server Database


The MVC project no longer connects directly to SQL Server. Instead, the MVC controllers call API service classes that use `HttpClient` to communicate with the backend API.

The backend API contains the business logic, repositories, database access, file upload handling, currency conversion, and authentication logic.



## Docker Container Setup

The project uses Docker Compose to run the full system.

The docker-compose.yml file starts three containers:

sql-server-db        - SQL Server database
glms-backend-api     - ASP.NET Core Web API backend
glms-frontend-web    - ASP.NET Core MVC frontend


The containers communicate using Docker service names:

glms-frontend-web → glms-backend-api → sql-server-db

The MVC frontend calls the API using:
http://glms-backend-api:8080/

The API connects to SQL Server using:
sql-server-db

This avoids using localhost inside the Docker network and allows the containers to communicate correctly.

## Prerequisites

Before running the project, make sure the following are installed:

* Docker Desktop
* Git
* .NET 10 SDK, only needed if running outside Docker
* A modern browser such as Chrome or Edge

Docker Desktop must be running before using the Docker commands.

---

## Cloning the Repository

Clone the repository and switch to the `main` branch:

powershell
git clone https://github.com/EMKNPM/programming-3a-prog7311-part-3-Sajana2101.git
cd programming-3a-prog7311-part-3-Sajana2101
git checkout main

If the repository was already cloned, pull the latest code from `main`:

(in powershell)
git checkout main
git pull origin main


---

## Running the Full System with Docker Compose

Open PowerShell or a terminal in the root folder of the project, where docker-compose.yml is located.

Build the containers:

(in powershell)
docker compose build


Start the full system:

docker compose up


Check that the containers are running:

docker compose ps


The following containers should be running:

sql-server-db
glms-backend-api
glms-frontend-web


---

## Application URLs

Once the containers are running, open the following URLs:

### MVC Frontend


http://localhost:8080


### Backend API Swagger

http://localhost:8081/swagger


### API Health Check

http://localhost:8081/health


The health check should return:

GLMS API is running


---

## Demo Login Details

Use the following login details when testing protected actions:


Username: admin
Password: Admin@123


Users can view records without logging in, but create, edit, and delete actions require authentication.

---


## Stopping the Containers

To stop the containers:

(in powershell)
docker compose down


To stop the containers and remove the Docker database volume:

docker compose down -v
---

## Running the Project Locally Without Docker

The project can also be run from Visual Studio.

Start both projects:


GLMS2.API2 
GLMS2        


The MVC frontend must be configured to call the API using the ApiSettings:BaseUrl value in appsettings.json.

For the final submission, Docker Compose is the recommended way to run the full system because it starts the database, API, and frontend together.

---

The integration tests cover examples such as:

* Calling
* Checking that the response status is 
* Confirming that JSON is returned
* Testing valid and invalid login responses
* Testing protected endpoints
* Testing a full client, contract, status update, and service request workflow
* Testing exchange rate retrieval

---

## Important Files

Key files for Part 3 include:


docker-compose.yml
.dockerignore
GLMS2/Dockerfile
GLMS2.API2/Dockerfile
GLMS2.API2/Program.cs
GLMS2/Program.cs
GLMS2.Test/IntegrationTests/ApiIntegrationTests.cs


---

YouTube Video: https://youtu.be/mpi0PVBhEdI
