<img align="left" width="116" src="https://raw.githubusercontent.com/adambarclay/mundane-hosting-aspnet/main/build/Mundane.png"/>

# Mundane.Hosting.AspNet

[![License: MIT](https://img.shields.io/github/license/adambarclay/mundane-hosting-aspnet?color=blue)](https://github.com/adambarclay/mundane-hosting-aspnet/blob/main/LICENSE) [![nuget](https://img.shields.io/nuget/v/Mundane.Hosting.AspNet)](https://www.nuget.org/packages/Mundane.Hosting.AspNet/) [![build](https://img.shields.io/github/workflow/status/adambarclay/mundane-hosting-aspnet/Build/main)](https://github.com/adambarclay/mundane-hosting-aspnet/actions?query=workflow%3ABuild+branch%3Amain) [![coverage](https://img.shields.io/codecov/c/github/adambarclay/mundane-hosting-aspnet/main)](https://codecov.io/gh/adambarclay/mundane-hosting-aspnet/branch/main)

Mundane is a lightweight "no magic" web framework for .NET.

This package enables a Mundane application to be hosted with ASP.NET.

See the [Mundane documentation](https://github.com/adambarclay/mundane) for more information.

## Getting Started

Install the [Mundane.Hosting.AspNet](https://www.nuget.org/packages/Mundane.Hosting.AspNet/) nuget package, then in your ASP.NET startup code call `app.UseMundane();` passing in the routing and dependencies configuration.

```c#
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var dependencies = new Dependencies(
            new Dependency<Configuration>(new Configuration(env)),
            new Dependency<DataRepository>(request => new DataRepositorySqlServer(
                request.Dependency<Configuration>().ConnectionString)));

        var routing = new Routing(
            routeConfiguration =>
            {
                routeConfiguration.Get("/", HomeController.HomePage);
                routeConfiguration.Get("/data/{id}", DataController.GetData);
                routeConfiguration.Post("/data/{id}", DataController.UpdateData);
            });

        app.UseMundane(dependencies, routing);
    }
```

## Executing Requests

Endpoints can be executed in a different part the ASP.NET pipeline by calling `MundaneMiddleware.ExecuteRequest()`. For example you may want to do custom error handling while still making use of the Mundane engine.

Passing the current `HttpContext` and the routing and dependencies configuration will execute the endpoint which matches the request.

```c#
    public static async ValueTask ExecuteRequest(
        HttpContext context,
        DependencyFinder dependencyFinder,
        Routing routing)
```

It is also possible to execute a specifc endpoint with:

```c#
    public static async ValueTask ExecuteRequest(
        HttpContext context,
        DependencyFinder dependencyFinder,
        MundaneEndpointDelegate endpoint,
        Dictionary<string, string> routeParameters)
```

The endpoint must be a `MundaneEndpointDelegate` which has the signature `ValueTask<Response> Endpoint(Request request)`. Any of the other Mundane endpoint signatures can be converted to a `MundaneEndpointDelegate` by calling `MundaneEndpoint.Create()` e.g.
```c#
    MundaneEndpoint.Create(() => Response.Ok(o => Write("Hello World!")));
```

Since there is no routing information in this version of `ExecuteRequest()`, you must also supply an appropriate `routeParameters` dictionary for the endpoint. When called as part of the pipeline, Mundane creates a dictionary of parameters captured from the URL, e.g. for the route `/my-endpoint/{id}`, called with `/my-endpoint/123`, Mundane passes `new Dictionary<string, string> { { "id", "123" } }` as `routeParameters`.

If the endpoint does not require route parameters, pass an empty dictionary: `new Dictionary<string, string>(0);`.
