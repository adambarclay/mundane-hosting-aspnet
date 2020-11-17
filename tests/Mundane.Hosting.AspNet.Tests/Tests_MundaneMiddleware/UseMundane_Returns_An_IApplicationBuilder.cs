using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_MundaneMiddleware
{
	[ExcludeFromCodeCoverage]
	public static class UseMundane_Returns_An_IApplicationBuilder
	{
		[Fact]
		public static void Identical_To_The_One_Passed_To_It()
		{
			var applicationBuilder = new Mock<IApplicationBuilder>(MockBehavior.Strict);

			var app = applicationBuilder.Object;

			applicationBuilder.Setup(o => o.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>())).Returns(app);

			var returnValue = app.UseMundane(new Routing(o => { }), new Dependencies());

			Assert.Same(app, returnValue);
		}
	}
}
