using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;

namespace LIC.Malone.Core
{
	public class Request
	{
		public string Url { get; set; }
		public string Method { get; set; }
		public string Token { get; set; }

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
			request.Accept = "text/xml";
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			request.Headers.Add(HttpRequestHeader.Authorization, string.Concat("Bearer ", Token));

			return request;
		}
	}
}
