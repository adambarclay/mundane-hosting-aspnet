using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace Mundane.Hosting.AspNet
{
	internal sealed class RequestAspNet : Request
	{
		private static readonly List<KeyValuePair<string, FileUpload>> EmptyFileList =
			new List<KeyValuePair<string, FileUpload>>(0);

		private static readonly List<KeyValuePair<string, string>>
			EmptyList = new List<KeyValuePair<string, string>>(0);

		private readonly HttpContext context;
		private readonly DependencyFinder dependencyFinder;
		private readonly RouteParameters routeParameters;

		internal RequestAspNet(HttpContext context, DependencyFinder dependencyFinder, RouteParameters routeParameters)
		{
			this.context = context;
			this.dependencyFinder = dependencyFinder;
			this.routeParameters = routeParameters;
		}

		public EnumerableCollection<KeyValuePair<string, string>> AllCookies
		{
			get
			{
				if (this.context.Request.Cookies.Count == 0)
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(RequestAspNet.EmptyList);
				}

				var list = new List<KeyValuePair<string, string>>(this.context.Request.Cookies.Count);

				foreach (var cookie in this.context.Request.Cookies)
				{
					list.Add(cookie);
				}

				return new EnumerableCollection<KeyValuePair<string, string>>(list);
			}
		}

		public EnumerableCollection<KeyValuePair<string, FileUpload>> AllFileParameters
		{
			get
			{
				if (!this.context.Request.HasFormContentType || this.context.Request.Form.Files.Count == 0)
				{
					return new EnumerableCollection<KeyValuePair<string, FileUpload>>(RequestAspNet.EmptyFileList);
				}

				var list = new List<KeyValuePair<string, FileUpload>>(this.context.Request.Form.Files.Count);

				foreach (var file in this.context.Request.Form.Files)
				{
					list.Add(new KeyValuePair<string, FileUpload>(file.Name, new FileUploadAspNet(file)));
				}

				return new EnumerableCollection<KeyValuePair<string, FileUpload>>(list);
			}
		}

		public EnumerableCollection<KeyValuePair<string, string>> AllFormParameters
		{
			get
			{
				if (!this.context.Request.HasFormContentType || this.context.Request.Form.Count == 0)
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(RequestAspNet.EmptyList);
				}

				var list = new List<KeyValuePair<string, string>>(this.context.Request.Form.Count);

				foreach ((var key, var value) in this.context.Request.Form)
				{
					list.Add(new KeyValuePair<string, string>(key, value));
				}

				return new EnumerableCollection<KeyValuePair<string, string>>(list);
			}
		}

		public EnumerableCollection<KeyValuePair<string, string>> AllHeaders
		{
			get
			{
				if (this.context.Request.Headers.Count == 0)
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(RequestAspNet.EmptyList);
				}

				var list = new List<KeyValuePair<string, string>>(this.context.Request.Headers.Count);

				foreach ((var key, var value) in this.context.Request.Headers)
				{
					list.Add(new KeyValuePair<string, string>(key.ToLowerInvariant(), value));
				}

				return new EnumerableCollection<KeyValuePair<string, string>>(list);
			}
		}

		public EnumerableCollection<KeyValuePair<string, string>> AllQueryParameters
		{
			get
			{
				if (this.context.Request.Query.Count == 0)
				{
					return new EnumerableCollection<KeyValuePair<string, string>>(RequestAspNet.EmptyList);
				}

				var list = new List<KeyValuePair<string, string>>(this.context.Request.Query.Count);

				foreach ((var key, var value) in this.context.Request.Query)
				{
					list.Add(new KeyValuePair<string, string>(key, value));
				}

				return new EnumerableCollection<KeyValuePair<string, string>>(list);
			}
		}

		public Stream Body
		{
			get
			{
				return this.context.Request.Body;
			}
		}

		public string Host
		{
			get
			{
				return this.context.Request.Host.ToString();
			}
		}

		public string Method
		{
			get
			{
				return this.context.Request.Method;
			}
		}

		public string Path
		{
			get
			{
				return this.context.Request.Path;
			}
		}

		public string PathBase
		{
			get
			{
				return this.context.Request.PathBase;
			}
		}

		public CancellationToken RequestAborted
		{
			get
			{
				return this.context.RequestAborted;
			}
		}

		public string Scheme
		{
			get
			{
				return this.context.Request.Scheme;
			}
		}

		public string Cookie(string cookieName)
		{
			if (cookieName is null)
			{
				throw new ArgumentNullException(nameof(cookieName));
			}

			if (this.context.Request.Cookies.TryGetValue(cookieName, out var value))
			{
				return value ?? string.Empty;
			}

			return string.Empty;
		}

		public bool CookieExists(string cookieName)
		{
			if (cookieName is null)
			{
				throw new ArgumentNullException(nameof(cookieName));
			}

			return this.context.Request.Cookies.ContainsKey(cookieName);
		}

		public T Dependency<T>()
			where T : notnull
		{
			return this.dependencyFinder.Find<T>(this);
		}

		public FileUpload File(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			if (this.context.Request.HasFormContentType)
			{
				var file = this.context.Request.Form.Files.GetFile(parameterName);

				if (file != null)
				{
					return new FileUploadAspNet(file);
				}
			}

			return FileUpload.Unknown;
		}

		public bool FileExists(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			return this.context.Request.HasFormContentType &&
				this.context.Request.Form.Files.GetFile(parameterName) != null;
		}

		public string Form(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			if (this.context.Request.HasFormContentType)
			{
				if (this.context.Request.Form.TryGetValue(parameterName, out var value))
				{
					return value.ToString() ?? string.Empty;
				}
			}

			return string.Empty;
		}

		public bool FormExists(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			return this.context.Request.HasFormContentType && this.context.Request.Form.ContainsKey(parameterName);
		}

		public string Header(string headerName)
		{
			if (headerName is null)
			{
				throw new ArgumentNullException(nameof(headerName));
			}

			if (this.context.Request.Headers.TryGetValue(headerName, out var value))
			{
				return value.ToString() ?? string.Empty;
			}

			return string.Empty;
		}

		public bool HeaderExists(string headerName)
		{
			if (headerName is null)
			{
				throw new ArgumentNullException(nameof(headerName));
			}

			return this.context.Request.Headers.ContainsKey(headerName);
		}

		public string Query(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			if (this.context.Request.Query.TryGetValue(parameterName, out var value))
			{
				return value.ToString() ?? string.Empty;
			}

			return string.Empty;
		}

		public bool QueryExists(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			return this.context.Request.Query.ContainsKey(parameterName);
		}

		public string Route(string parameterName)
		{
			if (parameterName is null)
			{
				throw new ArgumentNullException(nameof(parameterName));
			}

			return this.routeParameters.TryGetValue(parameterName, out var value) ? value : string.Empty;
		}
	}
}
