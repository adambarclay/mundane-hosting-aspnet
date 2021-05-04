using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class Path_Returns_A_Value
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task Which_Was_Passed_To_The_Constructor(EntryPoint entryPoint)
		{
			var path = "/" + Guid.NewGuid();

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.Create(responseStream, context => context.Request.Path = path),
					request => request.Path);

				Assert.Equal(path, result);
			}
		}
	}
}
