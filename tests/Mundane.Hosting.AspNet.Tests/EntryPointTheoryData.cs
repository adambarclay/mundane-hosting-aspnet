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
	[ExcludeFromCodeCoverage]
	internal sealed class EntryPointTheoryData
		: TheoryData<Func<HttpContext, string, string, MundaneEndpointDelegate, Task>>
	{
		public EntryPointTheoryData()
		{
			this.Add(
				async (context, method, path, endpoint) => await MundaneMiddleware.ExecuteRequest(
					context,
					new Routing(o => o.Endpoint(method, path, endpoint)),
					new Dependencies()));

			this.Add(
				async (context, method, path, endpoint) => await MundaneMiddleware.ExecuteRequest(
					context,
					endpoint,
					new Dictionary<string, string>(0),
					new Dependencies()));

			this.Add(
				async (context, method, path, endpoint) =>
					await new ApplicationBuilder(new Mock<IServiceProvider>(MockBehavior.Strict).Object!).UseMundane(
							new Routing(o => o.Endpoint(method, path, endpoint)),
							new Dependencies())
						.Build()
						.Invoke(context));
		}
	}
}
