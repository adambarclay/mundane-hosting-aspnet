using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_MundaneMiddleware
{
	[ExcludeFromCodeCoverage]
	public static class ExecuteRequest_With_Endpoint_Throws_ArgumentNullException
	{
		[Fact]
		public static async Task When_The_Context_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(
					null!,
					request => Task.FromResult(Response.Ok()),
					new Dictionary<string, string>(0),
					new Dependencies()));

			Assert.Equal("context", exception.ParamName!);
		}

		[Fact]
		public static async Task When_The_Dependency_Finder_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(
					new DefaultHttpContext(),
					request => Task.FromResult(Response.Ok()),
					new Dictionary<string, string>(0),
					null!));

			Assert.Equal("dependencyFinder", exception.ParamName!);
		}

		[Fact]
		public static async Task When_The_Endpoint_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(
					new DefaultHttpContext(),
					null!,
					new Dictionary<string, string>(0),
					new Dependencies()));

			Assert.Equal("endpoint", exception.ParamName!);
		}

		[Fact]
		public static async Task When_The_Route_Parameters_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(
					new DefaultHttpContext(),
					request => Task.FromResult(Response.Ok()),
					null!,
					new Dependencies()));

			Assert.Equal("routeParameters", exception.ParamName!);
		}
	}
}
