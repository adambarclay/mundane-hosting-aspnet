using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class Cookie_Returns_Empty_String
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_Cookie_Name_Is_Not_In_The_Collection(EntryPoint entryPoint)
	{
		var cookies = new Dictionary<string, string> { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } };

		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithCookies(responseStream, cookies),
				request => request.Cookie(Guid.NewGuid().ToString()));

			Assert.Equal(string.Empty, result);
		}
	}
}
