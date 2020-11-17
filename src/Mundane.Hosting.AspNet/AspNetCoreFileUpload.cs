using System.IO;
using Microsoft.AspNetCore.Http;

namespace Mundane.Hosting.AspNet
{
	internal sealed class AspNetCoreFileUpload : FileUpload
	{
		private readonly IFormFile formFile;

		internal AspNetCoreFileUpload(IFormFile formFile)
		{
			this.formFile = formFile;
		}

		public override string FileName
		{
			get
			{
				return this.formFile.FileName;
			}
		}

		public override long Length
		{
			get
			{
				return this.formFile.Length;
			}
		}

		public override string MediaType
		{
			get
			{
				return this.formFile.ContentType;
			}
		}

		public override Stream Open()
		{
			return this.formFile.OpenReadStream();
		}
	}
}
