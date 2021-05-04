using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class Body_Returns_A_Value
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task Which_Was_Passed_In_The_Http_Context(EntryPoint entryPoint)
		{
			var output = Guid.NewGuid().ToString();

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.Create(
						responseStream,
						context => context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(output))),
					request =>
					{
						using (var streamReader = new StreamReader(request.Body, Encoding.UTF8))
						{
							return streamReader.ReadToEnd();
						}
					});

				Assert.Equal(output, result);
			}
		}
	}
}
