# Bookcase
This is a pet project to practice building .NET Core application based on a simplified microservices architecture and Docker containers.

The application provides books/authors catalog, and service to create your own book collections called shelves.

## Architecture overview
The architecture proposes a microservice oriented architecture implementation with autonomous microservices. There are 3 microservices based on ASP.NET Core Web API, and 1 web application based on ASP.NET Core MVC. 

Microservices are implemented as a simple CRUD. Data is stored in SQL Server and MongoDB databases. Microservices list:
- "Catalog" is responsible for books/authors catalog.
- "Identity" is responsible for authentication/authorization.
- “Shelves” is responsible for users' owned book collections.

MVC communicates with microservices using HTTP protocol. The communication is based on gRPC and REST API using JSON format.

## Getting started

## Next to be implemented
The next things to be implemented are:
- Book CRUD operations in MVC.
- Integration events and event bus.
