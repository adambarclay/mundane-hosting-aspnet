using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class FormExists_Returns_True
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_Form_Parameter_Is_In_The_Collection(EntryPoint entryPoint)
	{
		var formParameter = Guid.NewGuid().ToString();

		var form = new Dictionary<string, string> { { formParameter, Guid.NewGuid().ToString() } };

		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithForm(responseStream, form),
				request => request.FormExists(formParameter));

			Assert.True(result);
		}
	}
}
