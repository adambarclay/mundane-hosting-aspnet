using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class Header_Returns_Empty_String
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task When_The_Header_Name_Is_Not_In_The_Collection(EntryPoint entryPoint)
		{
			var headerName = Guid.NewGuid().ToString();

			var headers = new Dictionary<string, string> { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } };

			await using (var responseStream = new MemoryStream())
			{
				var result = await Helper.Test(
					entryPoint,
					Helper.CreateWithHeaders(responseStream, headers),
					request => request.Header(headerName));

				Assert.Equal(string.Empty, result);
			}
		}
	}
}
