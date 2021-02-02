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
using Xunit;

namespace Mundane.Hosting.AspNet.Tests.Tests_MundaneMiddleware
{
	[ExcludeFromCodeCoverage]
	public static class When_Request_Processing_Has_Begun
	{
		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Body_Has_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var body = Guid.NewGuid().ToString();

			await using (var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(body)))
			{
				await using (var responseStream = new MemoryStream())
				{
					var context = new DefaultHttpContext();

					context.Request.Method = HttpMethod.Get;
					context.Request.Path = "/";
					context.Request.Body = requestStream;
					context.Response.Body = responseStream;

					await entryPoint.Invoke(
						context,
						HttpMethod.Get,
						"/",
						MundaneEndpointFactory.Create(request => Response.Ok(o => o.Write(request.Body))));

					context.Response.Body.Position = 0;

					using (var streamReader = new StreamReader(context.Response.Body, Encoding.UTF8))
					{
						Assert.Equal(body, await streamReader.ReadToEndAsync());
					}
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Cookies_Have_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var cookies = new[]
			{
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			};

			var cookieCollection = new Mock<IRequestCookieCollection>(MockBehavior.Strict);

			cookieCollection.Setup(o => o.Count)!.Returns(cookies.Count);

			cookieCollection.Setup(o => o.GetEnumerator())!.Returns(
				() => ((IEnumerable<KeyValuePair<string, string>>)cookies).GetEnumerator());

			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = HttpMethod.Get;
				context.Request.Path = "/";
				context.Response.Body = responseStream;
				context.Request.Cookies = cookieCollection.Object!;

				var output = new KeyValuePair<string, string>[cookies.Length];

				await entryPoint.Invoke(
					context,
					HttpMethod.Get,
					"/",
					MundaneEndpointFactory.Create(
						request =>
						{
							for (var i = 0; i < cookies.Length; ++i)
							{
								output[i] = new KeyValuePair<string, string>(
									cookies[i].Key,
									request.Cookie(cookies[i].Key));
							}

							return Response.Ok();
						}));

				for (var i = 0; i < cookies.Length; ++i)
				{
					Assert.Equal(cookies[i].Key, output[i].Key);
					Assert.Equal(cookies[i].Value, output[i].Value);
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_File_Uploads_Have_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var fileBytes = new[]
			{
				Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
				Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
				Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
			};

			var formFiles = new[]
			{
				new FormFile(
					new MemoryStream(fileBytes[0]),
					0,
					fileBytes[0].Length,
					Guid.NewGuid().ToString(),
					Guid.NewGuid().ToString())
				{
					Headers = new HeaderDictionary { { "Content-Type", Guid.NewGuid().ToString() } }
				},
				new FormFile(
					new MemoryStream(fileBytes[1]),
					0,
					fileBytes[1].Length,
					Guid.NewGuid().ToString(),
					Guid.NewGuid().ToString())
				{
					Headers = new HeaderDictionary { { "Content-Type", Guid.NewGuid().ToString() } }
				},
				new FormFile(
					new MemoryStream(fileBytes[2]),
					0,
					fileBytes[2].Length,
					Guid.NewGuid().ToString(),
					Guid.NewGuid().ToString())
				{
					Headers = new HeaderDictionary { { "Content-Type", Guid.NewGuid().ToString() } }
				}
			};

			var context = new DefaultHttpContext();

			context.Request.Method = HttpMethod.Get;
			context.Request.Path = "/";

			context.Request.Form = new FormCollection(
				new Dictionary<string, StringValues>(0),
				new FormFileCollection
				{
					formFiles[0],
					formFiles[1],
					formFiles[2]
				});

			var output = new FileUpload[formFiles.Length];

			await entryPoint.Invoke(
				context,
				HttpMethod.Get,
				"/",
				MundaneEndpointFactory.Create(
					request =>
					{
						for (var i = 0; i < formFiles.Length; ++i)
						{
							output[i] = request.File(formFiles[i].Name);
						}

						return Response.Ok();
					}));

			for (var i = 0; i < formFiles.Length; ++i)
			{
				Assert.Equal(formFiles[i].FileName, output[i].FileName);
				Assert.Equal(formFiles[i].Length, output[i].Length);
				Assert.Equal(formFiles[i].ContentType, output[i].MediaType);

				using (var streamReader = new StreamReader(output[i].Open()))
				{
					Assert.Equal(Encoding.UTF8.GetString(fileBytes[i]), await streamReader.ReadToEndAsync());
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Form_Parameters_Have_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var formParameters = new[]
			{
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			};

			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = HttpMethod.Get;
				context.Request.Path = "/";
				context.Response.Body = responseStream;

				context.Request.Form = new FormCollection(
					formParameters.ToDictionary(o => o.Key, o => new StringValues(o.Value)));

				var output = new KeyValuePair<string, string>[formParameters.Length];

				await entryPoint.Invoke(
					context,
					HttpMethod.Get,
					"/",
					MundaneEndpointFactory.Create(
						request =>
						{
							for (var i = 0; i < formParameters.Length; ++i)
							{
								output[i] = new KeyValuePair<string, string>(
									formParameters[i].Key,
									request.Form(formParameters[i].Key));
							}

							return Response.Ok();
						}));

				for (var i = 0; i < formParameters.Length; ++i)
				{
					Assert.Equal(formParameters[i].Key, output[i].Key);
					Assert.Equal(formParameters[i].Value, output[i].Value);
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Headers_Have_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var headers = new[]
			{
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			};

			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = HttpMethod.Get;
				context.Request.Path = "/";
				context.Response.Body = responseStream;
				context.Request.Headers.Add(headers[0].Key, headers[0].Value);
				context.Request.Headers.Add(headers[1].Key, headers[1].Value);
				context.Request.Headers.Add(headers[2].Key, headers[2].Value);

				var output = new KeyValuePair<string, string>[headers.Length];

				await entryPoint.Invoke(
					context,
					HttpMethod.Get,
					"/",
					MundaneEndpointFactory.Create(
						request =>
						{
							for (var i = 0; i < headers.Length; ++i)
							{
								output[i] = new KeyValuePair<string, string>(
									headers[i].Key,
									request.Header(headers[i].Key));
							}

							return Response.Ok();
						}));

				for (var i = 0; i < headers.Length; ++i)
				{
					Assert.Equal(headers[i], output[i]);
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Method_Has_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var method = Guid.NewGuid().ToString();

			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = method;
				context.Request.Path = "/";
				context.Response.Body = responseStream;

				await entryPoint.Invoke(
					context,
					method,
					"/",
					MundaneEndpointFactory.Create(request => Response.Ok(o => o.Write(request.Method))));

				context.Response.Body.Position = 0;

				using (var streamReader = new StreamReader(context.Response.Body, Encoding.UTF8))
				{
					Assert.Equal(method, await streamReader.ReadToEndAsync());
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Path_Has_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var path = "/" + Guid.NewGuid();

			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = HttpMethod.Get;
				context.Request.Path = path;
				context.Response.Body = responseStream;

				await entryPoint.Invoke(
					context,
					HttpMethod.Get,
					path,
					MundaneEndpointFactory.Create(request => Response.Ok(o => o.Write(request.Path))));

				context.Response.Body.Position = 0;

				using (var streamReader = new StreamReader(context.Response.Body, Encoding.UTF8))
				{
					Assert.Equal(path, await streamReader.ReadToEndAsync());
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task The_AspNetCore_Query_Parameters_Have_Been_Copied_To_The_Request(EntryPoint entryPoint)
		{
			var queryParameters = new[]
			{
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
				new KeyValuePair<string, string>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
			};

			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = HttpMethod.Get;
				context.Request.Path = "/";
				context.Response.Body = responseStream;

				context.Request.Query = new QueryCollection(
					queryParameters.ToDictionary(o => o.Key, o => new StringValues(o.Value)));

				var output = new KeyValuePair<string, string>[queryParameters.Length];

				await entryPoint.Invoke(
					context,
					HttpMethod.Get,
					"/",
					MundaneEndpointFactory.Create(
						request =>
						{
							for (var i = 0; i < queryParameters.Length; ++i)
							{
								output[i] = new KeyValuePair<string, string>(
									queryParameters[i].Key,
									request.Query(queryParameters[i].Key));
							}

							return Response.Ok();
						}));

				for (var i = 0; i < queryParameters.Length; ++i)
				{
					Assert.Equal(queryParameters[i].Key, output[i].Key);
					Assert.Equal(queryParameters[i].Value, output[i].Value);
				}
			}
		}

		[Theory]
		[ClassData(typeof(EntryPointTheoryData))]
		public static async Task There_Are_No_Form_Parameters_If_HasFormContentType_Is_False(EntryPoint entryPoint)
		{
			await using (var responseStream = new MemoryStream())
			{
				var context = new DefaultHttpContext();

				context.Request.Method = HttpMethod.Get;
				context.Request.Path = "/";
				context.Response.Body = responseStream;
				context.Request.ContentType = "application/json";

				var count = int.MaxValue;

				await entryPoint.Invoke(
					context,
					HttpMethod.Get,
					"/",
					MundaneEndpointFactory.Create(
						request =>
						{
							count = request.AllFormParameters.Count();

							return Response.Ok();
						}));

				Assert.Equal(0, count);
			}
		}
	}
}
