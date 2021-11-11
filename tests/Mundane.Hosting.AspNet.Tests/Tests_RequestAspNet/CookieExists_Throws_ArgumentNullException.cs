using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class CookieExists_Throws_ArgumentNullException
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_CookieName_Parameter_Is_Null(EntryPoint entryPoint)
	{
		var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () =>
			{
				var cookies = new Dictionary<string, string>(0);

				await using (var responseStream = new MemoryStream())
				{
					await Helper.Test(
						entryPoint,
						Helper.CreateWithCookies(responseStream, cookies),
						request => request.CookieExists(null!));
				}
			});

		Assert.Equal("cookieName", exception.ParamName!);
	}
}
