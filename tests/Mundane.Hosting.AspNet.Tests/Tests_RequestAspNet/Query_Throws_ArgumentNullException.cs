using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class Query_Throws_ArgumentNullException
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_ParameterName_Parameter_Is_Null(EntryPoint entryPoint)
	{
		var exception = await Assert.ThrowsAnyAsync<ArgumentNullException>(
			async () =>
			{
				await using (var responseStream = new MemoryStream())
				{
					await Helper.Test(
						entryPoint,
						Helper.CreateWithQuery(responseStream, new Dictionary<string, string>(0)),
						request => request.Query(null!));
				}
			});

		Assert.Equal("parameterName", exception.ParamName!);
	}
}
