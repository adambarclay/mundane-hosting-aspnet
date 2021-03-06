using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mundane.Hosting.AspNet
{
	/// <summary>The Mundane framework ASP.NET pipeline extension.</summary>
	public static class MundaneMiddleware
	{
		/// <summary>Executes a request.</summary>
		/// <param name="context">The ASP.NET HTTP context.</param>
		/// <param name="routing">The Mundane engine routing configuration.</param>
		/// <param name="dependencyFinder">The dependency finder.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="context"/>, <paramref name="routing"/> or <paramref name="dependencyFinder"/> is <see langword="null"/>.</exception>
		public static ValueTask ExecuteRequest(HttpContext context, Routing routing, DependencyFinder dependencyFinder)
		{
			if (context == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(context)));
			}

			if (routing == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(routing)));
			}

			if (dependencyFinder == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(dependencyFinder)));
			}

			return MundaneMiddleware.Execute(context, dependencyFinder, routing);
		}

		/// <summary>Executes a request.</summary>
		/// <param name="context">The ASP.NET HTTP context.</param>
		/// <param name="endpoint">The endpoint to invoke.</param>
		/// <param name="routeParameters">The parameters extracted from the route.</param>
		/// <param name="dependencyFinder">The dependency finder.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="context"/>, <paramref name="endpoint"/>, <paramref name="routeParameters"/> or <paramref name="dependencyFinder"/> is <see langword="null"/>.</exception>
		public static ValueTask ExecuteRequest(
			HttpContext context,
			MundaneEndpoint endpoint,
			Dictionary<string, string> routeParameters,
			DependencyFinder dependencyFinder)
		{
			if (context == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(context)));
			}

			if (endpoint == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(endpoint)));
			}

			if (routeParameters == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(routeParameters)));
			}

			if (dependencyFinder == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(dependencyFinder)));
			}

			return MundaneMiddleware.Execute(context, dependencyFinder, endpoint, routeParameters);
		}

		/// <summary>Adds the Mundane framework to the ASP.NET pipeline.</summary>
		/// <param name="app">The ASP.NET <see cref="IApplicationBuilder"/>.</param>
		/// <param name="routing">The Mundane engine routing configuration.</param>
		/// <param name="dependencyFinder">The dependency finder.</param>
		/// <returns>The same ASP.NET <see cref="IApplicationBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="app"/>, <paramref name="routing"/> or <paramref name="dependencyFinder"/> is <see langword="null"/>.</exception>
		public static IApplicationBuilder UseMundane(
			this IApplicationBuilder app,
			Routing routing,
			DependencyFinder dependencyFinder)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			if (routing == null)
			{
				throw new ArgumentNullException(nameof(routing));
			}

			if (dependencyFinder == null)
			{
				throw new ArgumentNullException(nameof(dependencyFinder));
			}

			app.Run(async context => await MundaneMiddleware.Execute(context, dependencyFinder, routing));

			return app;
		}

		private static async ValueTask Execute(HttpContext context, DependencyFinder dependencyFinder, Routing routing)
		{
			(var endpoint, var routeParameters) = routing.FindEndpoint(context.Request.Method, context.Request.Path);

			await MundaneMiddleware.Execute(context, dependencyFinder, endpoint, routeParameters);
		}

		private static async ValueTask Execute(
			HttpContext context,
			DependencyFinder dependencyFinder,
			MundaneEndpoint endpoint,
			EnumerableDictionary<string, string> routeParameters)
		{
			var request = new Request(
				context.Request.Method,
				context.Request.Path,
				routeParameters,
				RequestTransform.CreateHeaders(context.Request.Headers),
				context.Request.Body,
				RequestTransform.CreateQuery(context.Request.Query),
				RequestTransform.CreateForm(context.Request),
				RequestTransform.CreateCookies(context.Request.Cookies),
				RequestTransform.CreateFormFiles(context.Request),
				dependencyFinder,
				new RequestHost(context.Request.Scheme, context.Request.Host.ToString(), context.Request.PathBase),
				context.RequestAborted);

			var response = await MundaneEngine.ExecuteRequest(endpoint, request);

			context.Response.StatusCode = response.StatusCode;

			var headers = context.Response.Headers;

			foreach (var header in response.Headers)
			{
				headers[header.Name] = StringValues.Concat(headers[header.Name], header.Value);
			}

			await response.WriteBodyToStream(context.Response.Body);
		}
	}
}
