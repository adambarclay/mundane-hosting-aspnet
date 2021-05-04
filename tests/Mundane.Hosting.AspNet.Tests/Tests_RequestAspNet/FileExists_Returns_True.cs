using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class FileExists_Returns_True
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task When_The_File_Parameter_Is_In_The_Collection(EntryPoint entryPoint)
		{
			var fileParameter = Guid.NewGuid().ToString();

			var files = new[] { Helper.TestFormFile(fileParameter) };

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithFiles(responseStream, files),
					request => request.FileExists(fileParameter));

				Assert.True(result);
			}
		}
	}
}
