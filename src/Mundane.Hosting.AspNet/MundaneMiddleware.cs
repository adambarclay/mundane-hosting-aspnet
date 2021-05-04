using System;
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
		/// <param name="dependencyFinder">The dependency finder.</param>
		/// <param name="routing">The Mundane engine routing configuration.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="context"/>, <paramref name="routing"/> or <paramref name="dependencyFinder"/> is <see langword="null"/>.</exception>
		public static ValueTask ExecuteRequest(HttpContext context, DependencyFinder dependencyFinder, Routing routing)
		{
			if (context == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(context)));
			}

			if (dependencyFinder == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(dependencyFinder)));
			}

			if (routing == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(routing)));
			}

			return MundaneMiddleware.Execute(context, dependencyFinder, routing);
		}

		/// <summary>Executes a request.</summary>
		/// <param name="context">The ASP.NET HTTP context.</param>
		/// <param name="dependencyFinder">The dependency finder.</param>
		/// <param name="endpoint">The endpoint to invoke.</param>
		/// <param name="routeParameters">The parameters extracted from the route.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="context"/>, <paramref name="endpoint"/>, <paramref name="routeParameters"/> or <paramref name="dependencyFinder"/> is <see langword="null"/>.</exception>
		public static ValueTask ExecuteRequest(
			HttpContext context,
			DependencyFinder dependencyFinder,
			MundaneEndpoint endpoint,
			RouteParameters routeParameters)
		{
			if (context == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(context)));
			}

			if (dependencyFinder == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(dependencyFinder)));
			}

			if (endpoint == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(endpoint)));
			}

			if (routeParameters == null)
			{
				return ValueTask.FromException(new ArgumentNullException(nameof(routeParameters)));
			}

			return MundaneMiddleware.Execute(context, dependencyFinder, endpoint, routeParameters);
		}

		/// <summary>Adds the Mundane framework to the ASP.NET pipeline.</summary>
		/// <param name="app">The ASP.NET <see cref="IApplicationBuilder"/>.</param>
		/// <param name="dependencyFinder">The dependency finder.</param>
		/// <param name="routing">The Mundane engine routing configuration.</param>
		/// <returns>The same ASP.NET <see cref="IApplicationBuilder"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="app"/>, <paramref name="routing"/> or <paramref name="dependencyFinder"/> is <see langword="null"/>.</exception>
		public static IApplicationBuilder UseMundane(
			this IApplicationBuilder app,
			DependencyFinder dependencyFinder,
			Routing routing)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			if (dependencyFinder == null)
			{
				throw new ArgumentNullException(nameof(dependencyFinder));
			}

			if (routing == null)
			{
				throw new ArgumentNullException(nameof(routing));
			}

			app.Run(async context => await MundaneMiddleware.Execute(context, dependencyFinder, routing));

			return app;
		}

		private static ValueTask Execute(HttpContext context, DependencyFinder dependencyFinder, Routing routing)
		{
			(var endpoint, var routeParameters) = routing.FindEndpoint(context.Request.Method, context.Request.Path);

			return MundaneMiddleware.Execute(context, dependencyFinder, endpoint, routeParameters);
		}

		private static async ValueTask Execute(
			HttpContext context,
			DependencyFinder dependencyFinder,
			MundaneEndpoint endpoint,
			RouteParameters routeParameters)
		{
			var response = await MundaneEngine.ExecuteRequest(
				endpoint,
				new RequestAspNet(context, dependencyFinder, routeParameters));

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
