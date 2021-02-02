using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests
{
	public delegate ValueTask EntryPoint(HttpContext context, string method, string path, MundaneEndpoint endpoint);

	[ExcludeFromCodeCoverage]
	internal sealed class EntryPointTheoryData : TheoryData<EntryPoint>
	{
		public EntryPointTheoryData()
		{
			this.Add(
				(context, method, path, endpoint) => MundaneMiddleware.ExecuteRequest(
					context,
					new Routing(o => o.Endpoint(method, path, endpoint)),
					new Dependencies()));

			this.Add(
				(context, _, _, endpoint) => MundaneMiddleware.ExecuteRequest(
					context,
					endpoint,
					new Dictionary<string, string>(0),
					new Dependencies()));

			this.Add(
				(context, method, path, endpoint) => new ValueTask(
					new ApplicationBuilder(new Mock<IServiceProvider>(MockBehavior.Strict).Object!).UseMundane(
							new Routing(o => o.Endpoint(method, path, endpoint)),
							new Dependencies())
						.Build()
						.Invoke(context)));
		}
	}
}
