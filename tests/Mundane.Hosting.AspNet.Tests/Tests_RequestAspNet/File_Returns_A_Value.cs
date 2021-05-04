using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class File_Returns_A_Value
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task When_The_File_Parameter_Is_In_The_Collection(EntryPoint entryPoint)
		{
			var parameterName = Guid.NewGuid().ToString();
			var expectedFile = Helper.TestFormFile(parameterName);

			var files = new[] { expectedFile };

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithFiles(responseStream, files),
					request => request.File(parameterName));

				Assert.Equal(expectedFile.FileName, result.FileName);
				Assert.Equal(expectedFile.Length, result.Length);
				Assert.Equal(expectedFile.ContentType, result.MediaType);

				Assert.Equal(
					Helper.ReadStreamValue(expectedFile.OpenReadStream()),
					Helper.ReadStreamValue(result.Open()));
			}
		}
	}
}
