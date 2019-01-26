# TalosCore
TalosCore is a .Net Core controller generator. I decided to write it when I was creating microservices from scratch and I realized I was spending a lot of time implementing the same pattern over and over again.  I had to write the Entity Framework classes, but after that I generally just wanted accessors to them.  A perfect problem to solve with code-generation.

So I created a .Net Core console app that parses EF Core entities and generates the REST interface controllers and CQRS types to access them. It first finds the DbContext class and parses out the DbSet properties.  It takes that set of entities and then finds and parses the entity classes.  Then it generates controllers and CQRS types for the REST API.

The generated REST API includes GetList, Get, Create, and Update methods.  The resulting controller methods are not expected to be the final API for all projects, but is a baseline starting point that you can add to.

## How To Use TalosCore
See the project in the SampleProject directory for an example of what you should start with. It is a .Net Core MVC API project with a database context and some EF classes. You can easily create one using the Visual Studio project template for *ASP.NET Core Web Application*. Pick the application type API and then create your project. Add Entity Framework Core to the project using NuGet and write your database context and entity classes. After that, TalosCore will take care of the rest.

TalosCore is a .Net Core console application and is available everywhere as a .Net Core Global Tool. See the Microsoft docs [here](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for more information about .Net Core Global Tools.

You can install it from a console window by running this command:
```
dotnet tool install -g TalosCore
```
After installing, you can run TalosCore like this:
```
TalosCore .\SampleProject
```
I would appreciate any feedback about your experience using TalosCore or suggestions you have to improve it. Feel free to fork the project and send pull requests with bug fixes and enhancements.
