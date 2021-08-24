# ASP.NET Core 5.0 - Clean Architecture demo API


A demo API showing an implementation of Clean Architecture with ASP.NET Core 5.0.

This project allows basic CRUD operations on a SQL Server database using Dapper.

![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/1-Overview.png?raw=true "API overview image")

<br/>

## Clean Architecture
![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/2-CleanArchi.png?raw=true "Clean projects image")

<br/>

## Microsoft Identity
Users can register and authenticate to get a token. Thus is used to authorize access to the API (bearer).

A "401 Not authorized" responsed is returned if not authorized.

![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/3-Authorize.png?raw=true "Identity image")

<br/>

## API versioning

![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/4-API%20version.png?raw=true "API Version image")

<br/>

## CQRS

![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/5-CQRS.png?raw=true "CQRS image")

<br/>

## Packages used (ajouter les liens) :
- [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [MediatR](https://github.com/jbogard/MediatR) (Mediator pattern)
- [Automapper](https://github.com/AutoMapper/AutoMapper)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [EF Core](https://github.com/dotnet/efcore)
- [Dapper](https://github.com/DapperLib/Dapper)
- [Shouldly](https://github.com/shouldly/shouldly) (unit tests)
- [Moq](https://github.com/moq/moq4)
- [Serilog](https://github.com/serilog/serilog) + SerilogTimings for measuring methods performance
![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/6-Log.png?raw=true "Log image")

    Add traceability (who/when on modification operations in the database)
![Alt text](https://github.com/cvionnet/AspnetCore-Clean_architecture_demo/blob/main/_resources/7-Traceability.png?raw=true "Traceability image")


<br/>

## How to launch the application :
- clone / download project
- open with VS Studio 2019
- set API/Api project as "Startup project"
- launch the project
- navigate to : https://localhost:5001/swagger/index.html

ℹ️ : *database scripts are not include*
