using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class Host_Returns_A_Value
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task Which_Was_Passed_In_The_Http_Context(EntryPoint entryPoint)
		{
			var host = Guid.NewGuid().ToString();

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.Create(responseStream, context => context.Request.Host = new HostString(host)),
					request => request.Host);

				Assert.Equal(host, result);
			}
		}
	}
}
