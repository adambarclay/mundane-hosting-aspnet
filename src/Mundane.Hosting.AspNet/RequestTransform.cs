using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Mundane.Hosting.AspNet
{
	internal static class RequestTransform
	{
		private static readonly Dictionary<string, string> EmptyDictionary = new Dictionary<string, string>(0);

		private static readonly Dictionary<string, FileUpload> EmptyFileDictionary =
			new Dictionary<string, FileUpload>(0);

		internal static Dictionary<string, string> CreateCookies(IRequestCookieCollection cookies)
		{
			if (cookies.Count == 0)
			{
				return RequestTransform.EmptyDictionary;
			}

			var values = new Dictionary<string, string>(cookies.Count, StringComparer.OrdinalIgnoreCase);

			foreach ((var key, var value) in cookies)
			{
				values.Add(key, value);
			}

			return values;
		}

		internal static Dictionary<string, string> CreateForm(HttpRequest request)
		{
			if (!request.HasFormContentType || request.Form.Count == 0)
			{
				return RequestTransform.EmptyDictionary;
			}

			var values = new Dictionary<string, string>(request.Form.Count, StringComparer.OrdinalIgnoreCase);

			foreach ((var key, var value) in request.Form)
			{
				values.Add(key, value);
			}

			return values;
		}

		internal static Dictionary<string, FileUpload> CreateFormFiles(HttpRequest request)
		{
			if (!request.HasFormContentType || request.Form.Files.Count == 0)
			{
				return RequestTransform.EmptyFileDictionary;
			}

			var dictionary = new Dictionary<string, FileUpload>(request.Form.Files.Count);

			foreach (var file in request.Form.Files)
			{
				dictionary.Add(file.Name, new AspNetCoreFileUpload(file));
			}

			return dictionary;
		}

		internal static Dictionary<string, string> CreateHeaders(IHeaderDictionary headers)
		{
			if (headers.Count == 0)
			{
				return RequestTransform.EmptyDictionary;
			}

			var values = new Dictionary<string, string>(headers.Count, StringComparer.OrdinalIgnoreCase);

			foreach ((var key, var value) in headers)
			{
				values.Add(key.ToLowerInvariant(), value);
			}

			return values;
		}

		internal static Dictionary<string, string> CreateQuery(IQueryCollection query)
		{
			if (query.Count == 0)
			{
				return RequestTransform.EmptyDictionary;
			}

			var values = new Dictionary<string, string>(query.Count, StringComparer.OrdinalIgnoreCase);

			foreach ((var key, var value) in query)
			{
				values.Add(key, value);
			}

			return values;
		}
	}
}
