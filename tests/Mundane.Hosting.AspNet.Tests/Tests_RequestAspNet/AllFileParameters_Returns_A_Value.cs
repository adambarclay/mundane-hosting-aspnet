using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class AllFileParameters_Returns_A_Value
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task Which_Is_Empty_When_No_Values_Are_Passed_In_The_Http_Context(EntryPoint entryPoint)
		{
			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithFiles(responseStream, Array.Empty<FormFile>()),
					request => request.AllFileParameters);

				Assert.Empty(result);
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task Which_Was_Passed_In_The_Http_Context(EntryPoint entryPoint)
		{
			var files = new[]
			{
				Helper.TestFormFile(Guid.NewGuid().ToString()),
				Helper.TestFormFile(Guid.NewGuid().ToString()),
				Helper.TestFormFile(Guid.NewGuid().ToString())
			};

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithFiles(responseStream, files),
					request => request.AllFileParameters);

				var index = 0;

				foreach ((var parameterName, var file) in result)
				{
					var expectedFile = files[index++];

					Assert.Equal(expectedFile.Name, parameterName);
					Assert.Equal(expectedFile.FileName, file.FileName);
					Assert.Equal(expectedFile.Length, file.Length);
					Assert.Equal(expectedFile.ContentType, file.MediaType);

					Assert.Equal(
						Helper.ReadStreamValue(expectedFile.OpenReadStream()),
						Helper.ReadStreamValue(file.Open()));
				}
			}
		}
	}
}
