# TalosCore
TalosCore is a dotnet Core controller generator. I decided to write it when I was creating microservices from scratch and I realized I was spending a lot of time implementing the same pattern of code based on simple schema.

It uses definitions of Entity Framework Core entities and generates REST interface controllers and CQRS types to access them. It first finds the DbContext class and parses out the DbSet properties. It takes that set of entities and then finds and parses the entity classes.  then it generates the controller files as a REST API.

The API is a simple pattern of GetList, Get, Create, and Update--basic CRUD operations (though Delete isn't there just yet).

The resulting controller methods are not expected to be the final API for all projects, but is a baseline starting point that you can enhance to add relationships and logic to.

## How To Use TalosCore
See the project in the SampleProject directory for an example of what you should start with. It is a dotnet Core MVC project with a database context and some entities defined. You can easily create one using the Visual Studio project template for *ASP.NET Core Web Application*. Pick the application type API and then create your project. Add Entity Framework Core to the project and add your database context and entity classes. After that, TalosCore will take care of the rest.

TalosCore is a dotnet Core console application.  As such, it should be run from the command line like this:
```
dotnet <path to TalosCore.dll> <path to project with EF entities>
```
Example:
```
C:\git\TalosCore> dotnet .\taloscore\bin\release\netcoreapp2.1\TalosCore.dll .\SampleProject
```
I would appreciate any feedback or suggestions you have on your experience using TalosCore, and feel free to fork the project and send pull requests with bug fixes and enhancements.
