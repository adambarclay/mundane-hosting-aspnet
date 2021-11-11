using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_RequestAspNet;

[ExcludeFromCodeCoverage]
public static class Dependency_Returns_A_Value
{
	private interface TestDependencyBase
	{
	}

	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task Returned_By_The_Dependency_Finder(EntryPoint entryPoint)
	{
		await using (var responseStream = new MemoryStream())
		{
			(var originalRequest, var result) = await Helper.Test(
				entryPoint,
				Helper.Create(responseStream),
				new Dependencies(new Dependency<TestDependencyBase>(r => new TestDependencyWithRequest(r))),
				request => (request, request.Dependency<TestDependencyBase>()));

			Assert.IsType<TestDependencyWithRequest>(result);
			Assert.Same(originalRequest, ((TestDependencyWithRequest)result).Request);
		}
	}

	private sealed class TestDependencyWithRequest : TestDependencyBase
	{
		internal TestDependencyWithRequest(Request request)
		{
			this.Request = request;
		}

		internal Request Request { get; }
	}
}
