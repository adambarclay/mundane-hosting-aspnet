using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Mundane.Hosting.AspNet.Tests;

[ExcludeFromCodeCoverage]
internal static class Helper
{
	public static HttpContext CreateWithRoute(MemoryStream responseStream, Dictionary<string, string> routeParameters)
	{
		var context = Helper.Create(responseStream);

		var route = "/" + string.Join("/", routeParameters.Keys.Select(o => "{" + o + "}"));
		var path = "/" + string.Join("/", routeParameters.Values);

		context.Items["route"] = route;
		context.Request.Path = path;

		return context;
	}

	internal static HttpContext Create(Stream responseStream)
	{
		return new DefaultHttpContext
		{
			Request =
			{
				Method = HttpMethod.Get,
				Path = "/"
			},
			Response = { Body = responseStream }
		};
	}

	internal static HttpContext Create(Stream responseStream, Action<HttpContext> modifyContext)
	{
		var context = Helper.Create(responseStream);

		modifyContext(context);

		return context;
	}

	internal static HttpContext CreateWithCookies(Stream responseStream, Dictionary<string, string> cookies)
	{
		var cookieCollection = new Mock<IRequestCookieCollection>(MockBehavior.Strict);

		cookieCollection.Setup(o => o.Count)!.Returns(cookies.Count);

		cookieCollection.Setup(o => o.ContainsKey(It.IsAny<string>()!))!.Returns<string>(cookies.ContainsKey);

		cookieCollection.Setup(o => o.TryGetValue(It.IsAny<string>()!, out It.Ref<string>.IsAny!))!.Returns(
			(string key, out string value) => cookies.TryGetValue(key, out value!));

		cookieCollection.Setup(o => o.GetEnumerator())!.Returns(
			() => ((IEnumerable<KeyValuePair<string, string>>)cookies).GetEnumerator());

		var context = Helper.Create(responseStream);

		context.Request.Cookies = cookieCollection.Object!;

		return context;
	}

	internal static HttpContext CreateWithFiles(MemoryStream responseStream, IEnumerable<FormFile> files)
	{
		var context = Helper.Create(responseStream);

		var formFileCollection = new FormFileCollection();

		formFileCollection.AddRange(files);

		context.Request.Form = new FormCollection(null, formFileCollection);

		return context;
	}

	internal static HttpContext CreateWithForm(
		MemoryStream responseStream,
		IEnumerable<KeyValuePair<string, string>> form)
	{
		var context = Helper.Create(responseStream);

		context.Request.Form = new FormCollection(form.ToDictionary(o => o.Key, o => new StringValues(o.Value)));

		return context;
	}

	internal static HttpContext CreateWithHeaders(
		MemoryStream responseStream,
		IEnumerable<KeyValuePair<string, string>> headers)
	{
		var context = Helper.Create(responseStream);

		foreach ((var key, var value) in headers)
		{
			context.Request.Headers[key] = value;
		}

		return context;
	}

	internal static HttpContext CreateWithQuery(
		MemoryStream responseStream,
		IEnumerable<KeyValuePair<string, string>> query)
	{
		var context = Helper.Create(responseStream);

		context.Request.Query = new QueryCollection(query.ToDictionary(o => o.Key, o => new StringValues(o.Value)));

		return context;
	}

	internal static string ReadStreamValue(Stream stream)
	{
		using (var streamReader = new StreamReader(stream, null, true, -1, true))
		{
			return streamReader.ReadToEnd();
		}
	}

	internal static ValueTask<T> Test<T>(EntryPoint entryPoint, HttpContext context, Func<Request, T> fetchValue)
	{
		return Helper.Test(entryPoint, context, new Dependencies(), fetchValue);
	}

	internal static async ValueTask<T> Test<T>(
		EntryPoint entryPoint,
		HttpContext context,
		DependencyFinder dependencyFinder,
		Func<Request, T> fetchValue)
	{
		T returnValue = default!;

		await entryPoint.Invoke(
			context,
			dependencyFinder,
			request =>
			{
				try
				{
					returnValue = fetchValue(request);
				}
				catch (Exception exception)
				{
					return ValueTask.FromException<Response>(exception);
				}

				return ValueTask.FromResult(Response.Ok());
			});

		return returnValue;
	}

	internal static FormFile TestFormFile(string parameterName)
	{
		var value = new MemoryStream(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

		return new FormFile(value, 0, value.Length, parameterName, Guid.NewGuid().ToString())
		{
			Headers = new HeaderDictionary { { "Content-Type", Guid.NewGuid().ToString() } }
		};
	}
}
