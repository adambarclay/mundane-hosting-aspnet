using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests
{
	public delegate ValueTask EntryPoint(HttpContext context, DependencyFinder dependencies, MundaneEndpoint endpoint);

	[ExcludeFromCodeCoverage]
	internal sealed class EntryPointTheoryData : TheoryData<EntryPoint>
	{
		public EntryPointTheoryData()
		{
			this.Add(
				(context, dependencies, endpoint) => MundaneMiddleware.ExecuteRequest(
					context,
					dependencies,
					EntryPointTheoryData.CreateRouting(context, endpoint)));

			this.Add(
				(context, dependencies, endpoint) =>
				{
					var routeParameters = EntryPointTheoryData.CreateRouting(context, endpoint)
						.FindEndpoint(context.Request.Method, context.Request.Path)
						.RouteParameters;

					return MundaneMiddleware.ExecuteRequest(context, dependencies, endpoint, routeParameters);
				});

			this.Add(
				(context, dependencies, endpoint) => new ValueTask(
					new ApplicationBuilder(new Mock<IServiceProvider>(MockBehavior.Strict).Object!).UseMundane(
							dependencies,
							EntryPointTheoryData.CreateRouting(context, endpoint))
						.Build()
						.Invoke(context)));
		}

		private static Routing CreateRouting(HttpContext context, MundaneEndpoint endpoint)
		{
			return new Routing(
				o => o.Endpoint(
					context.Request.Method,
					context.Items["route"] as string ?? context.Request.Path,
					endpoint));
		}
	}
}
