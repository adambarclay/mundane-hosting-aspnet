using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class Route_Returns_Empty_String
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task When_The_Route_Parameter_Is_Not_In_The_Collection(EntryPoint entryPoint)
		{
			var routeParameter = Guid.NewGuid().ToString();

			var routeParameters =
				new Dictionary<string, string> { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } };

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithRoute(responseStream, routeParameters),
					new Dependencies(),
					request => request.Route(routeParameter));

				Assert.Equal(string.Empty, result);
			}
		}
	}
}
