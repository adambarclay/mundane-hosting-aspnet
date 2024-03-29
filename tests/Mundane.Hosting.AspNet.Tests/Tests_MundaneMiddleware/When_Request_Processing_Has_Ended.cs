using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_MundaneMiddleware;

[ExcludeFromCodeCoverage]
public static class When_Request_Processing_Has_Ended
{
	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task The_Response_Body_Has_Been_Written_To_The_AspNetCore_Body(EntryPoint entryPoint)
	{
		var output = Guid.NewGuid().ToString();

		await using (var responseStream = new MemoryStream())
		{
			var context = Helper.Create(responseStream);

			await entryPoint.Invoke(
				context,
				new Dependencies(),
				MundaneEndpointFactory.Create(() => Response.Ok(o => o.Write(output))));

			context.Response.Body.Position = 0;

			using (var streamReader = new StreamReader(context.Response.Body, Encoding.UTF8))
			{
				Assert.Equal(output, await streamReader.ReadToEndAsync());
			}
		}
	}

	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task The_Response_Cookies_Have_Been_Copied_To_The_AspNetCore_Headers(EntryPoint entryPoint)
	{
		var cookies = new[]
		{
			new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
			new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
			new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
		};

		await using (var responseStream = new MemoryStream())
		{
			var context = Helper.Create(responseStream);

			await entryPoint.Invoke(
				context,
				new Dependencies(),
				MundaneEndpointFactory.Create(
					() => Response.Ok()
						.AddHeader(HeaderValue.SessionCookie(cookies[0].Key, cookies[0].Value))
						.AddHeader(HeaderValue.SessionCookie(cookies[1].Key, cookies[1].Value))
						.AddHeader(HeaderValue.SessionCookie(cookies[2].Key, cookies[2].Value))));

			var responseCookies = context.Response.Headers["set-cookie"];

			for (var i = 0; i < cookies.Length; i++)
			{
				var expected = cookies[i].Key + "=" + cookies[i].Value;

				Assert.StartsWith(expected, responseCookies[i]!, StringComparison.Ordinal);
			}
		}
	}

	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task The_Response_Headers_Have_Been_Copied_To_The_AspNetCore_Headers(EntryPoint entryPoint)
	{
		var headers = new[]
		{
			new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
			new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
			new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
		};

		await using (var responseStream = new MemoryStream())
		{
			var context = Helper.Create(responseStream);

			await entryPoint.Invoke(
				context,
				new Dependencies(),
				MundaneEndpointFactory.Create(
					() => Response.Ok()
						.AddHeader(new HeaderValue(headers[0].Key, headers[0].Value))
						.AddHeader(new HeaderValue(headers[1].Key, headers[1].Value))
						.AddHeader(new HeaderValue(headers[2].Key, headers[2].Value))));

			Assert.Equal(headers.Length, context.Response.Headers.Count);

			foreach ((var key, var value) in headers)
			{
				Assert.Equal(value, context.Response.Headers[key]);
			}
		}
	}

	[Theory]
	[ClassData(typeof(EntryPointTheoryData))]
	public static async Task The_Response_Status_Has_Been_Copied_To_The_AspNetCore_Status(EntryPoint entryPoint)
	{
		const int statusCode = 12345;

		await using (var responseStream = new MemoryStream())
		{
			var context = Helper.Create(responseStream);

			await entryPoint.Invoke(
				context,
				new Dependencies(),
				MundaneEndpointFactory.Create(() => new Response(statusCode)));

			Assert.Equal(statusCode, context.Response.StatusCode);
		}
	}
}
