using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class AllQueryParameters_Returns_A_Value
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task Which_Is_Empty_When_No_Values_Are_Passed_In_The_Http_Context(EntryPoint entryPoint)
	{
		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithQuery(responseStream, new Dictionary<string, string>(0)),
				request => request.AllQueryParameters);

			Assert.Empty(result);
		}
	}

	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task Which_Was_Passed_In_The_Http_Context(EntryPoint entryPoint)
	{
		var query = new Dictionary<string, string>
		{
			{ Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
			{ Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
			{ Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
		};

		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithQuery(responseStream, query),
				request => request.AllQueryParameters);

			Assert.Equal(query, result);
		}
	}
}
