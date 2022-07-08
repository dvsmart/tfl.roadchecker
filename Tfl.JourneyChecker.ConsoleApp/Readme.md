### TFL Road Checker App

The project has been developed in .Net 6 Console App using TFL API to get the road statuses by passing road Id as input parameter.

### Overview

The Tfl.JourneyChecker Solution consists of two projects,

1. Tfl.JourneyChecker.ConsoleApp
2. Tfl.JourneyChecker.UnitTests

The TFL journey checker Console App takes the Road Id as an input arguements to return the road status for the given road Id. If not, appropriate response will be returned.
This App invokes an TFL API endpoint using HttpClient to get the road status and display in the console App.This app uses TFL App Id and app key to authenticate TFL endpoint which can be given in the AppSettings.Json file of the Console App Project.

The following configurations are being provided in AppSettings.Json file which provisioning to have a different version of API endpoint (API Base URL), App Id and App Key.

### Steps to run the console App

1. Open the Tfl.JourneyChecker solution on visual studio 2022 and build the solution.
Open a Command prompt as admin.

2. To run Journey checker app, Navigate to the Tfl.JourneyPlanner.ConsoleApp directory.
Run  "dotnet run A2" or "dotnet run A22"

### Steps to run the Tests

1. Navigate to the Tfl.JourneyPlanner.ConsoleApp.Unit.Tests directory.
2. Run dotnet test

## Architecture Design & Patterns Usage

#### CQRS

I have used CQRS architectural (Command query responsibility segregation) pattern to query the road status for read operation, which will seperate the write any data if required in the future 
for seperation of operations.

#### Factory pattern:

I have used a Factory pattern in this app keeping in mind that we can just have a new product without modifying the existing code in the same project.
Let's say, if we have a requirement to get the status of tube, location, It would be helpful that we can create a new class called RailJourneyChecker, etc to handle different types of transport.

NOTE: I've resolved the object using autofac instead of creating factory concrete class to create the object. 

#### Mediator Pattern:

I have used Mediator Behavioural pattern with Nuget Package to have a loosely coupled communication between query handlers. It reduces tightly coupled communication issues if we have multiple services to be invoked.

#### Serilog

Used Serilog for logging any errors in the app and writes into the log folder in the current app directory
 
#### Tech stack
   Net6.0 
   C#
   
#### Libraries used in Console App repo
  Autofac
  Autofac.Extensions.DependencyInjection
  Microsoft.AspNetCore.WebUtilities
  Microsoft.Extensions.Configuration
  Microsoft.Extensions.DependencyInjection
  Microsoft.Extensions.Logging
  Newtonsoft.Json
  Serilog
  MediatR
  MediatR.Extensions.Microsoft.DependencyInjection

#### Libraries used in Test project Repo
  MediatR
  MediatR.Extensions.Microsoft.DependencyInjection
  Microsoft.NET.Test.Sdk
  Moq
  Shouldly
  xunit
  xunit.runner.visualstudio

### Tools
 Visual studio 2022

#### Application Type
 Console App 

#### Uses cases covered

The Following use cases have been covered in this app,

   1. Given a valid road ID is specified
    When the client is run
    Then the road ‘displayName’ should be displayed

   2. Given a valid road ID is specified
    When the client is run
    Then the road ‘statusSeverity’ should be displayed as ‘Road Status’

   3. Given a valid road ID is specified
    When the client is run
    Then the road ‘statusSeverityDescription’ should be displayed as ‘Road Status Description’

   4. Given an invalid road ID is specified
    When the client is run
    Then the application should return an informative error

   5. Given an invalid road ID is specified
    When the client is run
    Then the application should exit with a non-zero System Error code