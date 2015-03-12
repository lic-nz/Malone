using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DotNetOpenAuth.OAuth2;
using LIC.Malone.Core.Authentication.OAuth;

namespace LIC.Malone.Core
{
	public class ApiClient
	{
		public string Send(Request request)
		{
			return Send(request.ToHttpWebRequest());
		}

		public string Send(HttpWebRequest request)
		{
			var response = request.GetResponse() as HttpWebResponse;

			if (response == null)
				throw new Exception(string.Format("Failed to get response for URL '{0}'", request.RequestUri));

			var stream = response.GetResponseStream();

			if (stream == null)
				throw new Exception(string.Format("Failed to get stream for URL '{0}'", request.RequestUri));

			var s = new StreamReader(stream).ReadToEnd();

			return s;
		}
	}
}
