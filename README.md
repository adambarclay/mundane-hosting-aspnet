# Mundane.Hosting.AspNet

[![License: MIT](https://img.shields.io/github/license/adambarclay/mundane-hosting-aspnet?color=blue)](https://github.com/adambarclay/mundane-hosting-aspnet/blob/main/LICENSE) [![build](https://img.shields.io/github/workflow/status/adambarclay/mundane-hosting-aspnet/Build/main)](https://github.com/adambarclay/mundane-hosting-aspnet/actions?query=workflow%3ABuild+branch%3Amain) [![coverage](https://img.shields.io/codecov/c/github/adambarclay/mundane-hosting-aspnet/main)](https://codecov.io/gh/adambarclay/mundane-hosting-aspnet/branch/main)

Mundane is a lightweight "no magic" web framework for .NET.

This package enables a Mundane application to be hosted with ASP.NET.

See the [Mundane documentation](https://github.com/adambarclay/mundane/blob/main/README.md) for more information.

## Getting Started

In Startup.cs, configure the application routing and dependencies and call `UseMundane()`.

`UseMundane()` should be the last middleware component added to the pipeline.

```c#
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var routing = new Routing(
            routeConfiguration =>
            {
                routeConfiguration.Get("/", HomeController.HomePage);
                routeConfiguration.Get("/data/{id}", DataController.GetData);
                routeConfiguration.Post("/data/{id}", DataController.UpdateData);
            });

        var dependencies = new Dependencies(
            new Dependency<Configuration>(new Configuration(env)),
            new Dependency<DataRepository>(request => new DataRepositorySqlServer(request.Dependency<Configuration>().ConnectionString)));

        app.UseMundane(routing, dependencies);
    }
```

## Executing Requests

Endpoints can be executed outside of the ASP.NET pipeline by calling `MundaneMiddleware.ExecuteRequest()`.

Passing the current `HttpContext` and the routing and dependencies configuration will execute the endpoint which matches the request.

```c#
    public static async Task ExecuteRequest(
        HttpContext context,
        Routing routing,
        DependencyFinder dependencyFinder)
```

It is also possible to execute a specifc endpoint. The endpoint must be a `MundaneEndpointDelegate` which has the signature `Task<Response> Endpoint(Request request)`.

```c#
    public static async Task ExecuteRequest(
        HttpContext context,
        MundaneEndpointDelegate endpoint,
        Dictionary<string, string> routeParameters,
        DependencyFinder dependencyFinder)
```
