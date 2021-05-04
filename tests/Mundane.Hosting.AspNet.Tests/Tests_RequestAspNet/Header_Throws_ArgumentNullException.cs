using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet
{
	[ExcludeFromCodeCoverage]
	public static class Header_Throws_ArgumentNullException
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task When_The_HeaderName_Parameter_Is_Null(EntryPoint entryPoint)
		{
			var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
				async () =>
				{
					await using (var responseStream = new MemoryStream())
					{
						await Helper.Test(
							entryPoint,
							Helper.CreateWithHeaders(responseStream, new Dictionary<string, string>(0)),
							request => request.Header(null!));
					}
				});

			Assert.Equal("headerName", exception.ParamName!);
		}
	}
}
