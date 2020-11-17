using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Moq;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_MundaneMiddleware
{
	[ExcludeFromCodeCoverage]
	public static class UseMundane_Throws_ArgumentNullException
	{
		[Fact]
		public static void When_The_App_Parameter_Is_Null()
		{
			var exception = Assert.ThrowsAny<ArgumentNullException>(
				() => MundaneMiddleware.UseMundane(null!, new Routing(o => { }), new Dependencies()));

			Assert.Equal("app", exception.ParamName);
		}

		[Fact]
		public static void When_The_Dependency_Finder_Parameter_Is_Null()
		{
			var exception = Assert.ThrowsAny<ArgumentNullException>(
				() => new Mock<IApplicationBuilder>(MockBehavior.Strict).Object.UseMundane(
					new Routing(o => { }),
					null!));

			Assert.Equal("dependencyFinder", exception.ParamName);
		}

		[Fact]
		public static void When_The_Routing_Parameter_Is_Null()
		{
			var exception = Assert.ThrowsAny<ArgumentNullException>(
				() => new Mock<IApplicationBuilder>(MockBehavior.Strict).Object.UseMundane(null!, new Dependencies()));

			Assert.Equal("routing", exception.ParamName);
		}
	}
}
