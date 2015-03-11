using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LIC.Malone.Core
{
	public class Request
	{
		public string Url { get; set; }
		public string Method { get; set; }

		public HttpWebRequest ToHttpWebRequest()
		{
			Uri url;
			if (!Uri.TryCreate(Url, UriKind.Absolute, out url))
				throw new Exception(string.Format("Failed to create a URI from the string '{0}'.", Url));

			var request = (HttpWebRequest)WebRequest.Create(url);

			if (request == null)
				throw new Exception("Failed to create HttpWebRequest.");

			request.Method = Method;
			//request.ContentType = contentType;
			request.AllowAutoRedirect = true;
			//request.Accept = accept;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			return request;
		}
	}
}
