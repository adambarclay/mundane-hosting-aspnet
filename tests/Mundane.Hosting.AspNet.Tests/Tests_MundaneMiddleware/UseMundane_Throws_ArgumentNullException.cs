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
				() => MundaneMiddleware.UseMundane(null!, new Dependencies(), new Routing(_ => { })));

			Assert.Equal("app", exception.ParamName!);
		}

		[Fact]
		public static void When_The_Dependency_Finder_Parameter_Is_Null()
		{
			var exception = Assert.ThrowsAny<ArgumentNullException>(
				() => new Mock<IApplicationBuilder>(MockBehavior.Strict).Object!.UseMundane(
					null!,
					new Routing(_ => { })));

			Assert.Equal("dependencyFinder", exception.ParamName!);
		}

		[Fact]
		public static void When_The_Routing_Parameter_Is_Null()
		{
			var exception = Assert.ThrowsAny<ArgumentNullException>(
				() => new Mock<IApplicationBuilder>(MockBehavior.Strict).Object!.UseMundane(new Dependencies(), null!));

			Assert.Equal("routing", exception.ParamName!);
		}
	}
}
