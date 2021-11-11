using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class Form_Returns_A_Value
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task When_The_Form_Parameter_Is_In_The_Collection(EntryPoint entryPoint)
	{
		var formParameter = Guid.NewGuid().ToString();
		var formValue = Guid.NewGuid().ToString();

		var form = new Dictionary<string, string> { { formParameter, formValue } };

		await using (var responseStream = new MemoryStream())
		{
			var result = await Helper.Test(
				entryPoint,
				Helper.CreateWithForm(responseStream, form),
				request => request.Form(formParameter));

			Assert.Equal(formValue, result);
		}
	}
}
