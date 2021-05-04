using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class File_Returns_Unknown_File
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task When_The_File_Parameter_Is_Not_In_The_Collection(EntryPoint entryPoint)
		{
			var parameterName = Guid.NewGuid().ToString();

			var files = new[] { Helper.TestFormFile(Guid.NewGuid().ToString()) };

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithFiles(responseStream, files),
					request => request.File(parameterName));

				Assert.Equal(FileUpload.Unknown, result);
			}
		}
	}
}
