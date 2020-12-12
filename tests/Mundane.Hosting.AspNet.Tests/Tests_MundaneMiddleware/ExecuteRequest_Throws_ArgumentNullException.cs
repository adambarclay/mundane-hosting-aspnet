using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_MundaneMiddleware
{
	[ExcludeFromCodeCoverage]
	public static class ExecuteRequest_Throws_ArgumentNullException
	{
		[Fact]
		public static async Task When_The_Context_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(null!, new Routing(o => { }), new Dependencies()));

			Assert.Equal("context", exception.ParamName!);
		}

		[Fact]
		public static async Task When_The_Dependency_Finder_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(
					new DefaultHttpContext(),
					new Routing(o => { }),
					null!));

			Assert.Equal("dependencyFinder", exception.ParamName!);
		}

		[Fact]
		public static async Task When_The_Routing_Parameter_Is_Null()
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () => await MundaneMiddleware.ExecuteRequest(
					new DefaultHttpContext(),
					null!,
					new Dependencies()));

			Assert.Equal("routing", exception.ParamName!);
		}
	}
}
