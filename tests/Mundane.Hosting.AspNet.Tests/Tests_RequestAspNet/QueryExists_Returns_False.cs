using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class QueryExists_Returns_False
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_Query_Parameter_Is_Not_In_The_Collection(EntryPoint entryPoint)
	{
		var queryParameter = Guid.NewGuid().ToString();

		var query = new Dictionary<string, string> { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } };

		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithQuery(responseStream, query),
				request => request.QueryExists(queryParameter));

			Assert.False(result);
		}
	}
}
