using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class CookieExists_Returns_True
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_Cookie_Name_Is_In_The_Collection(EntryPoint entryPoint)
	{
		var cookieName = Guid.NewGuid().ToString();

		var cookies = new Dictionary<string, string> { { cookieName, Guid.NewGuid().ToString() } };

		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithCookies(responseStream, cookies),
				request => request.CookieExists(cookieName));

			Assert.True(result);
		}
	}
}
