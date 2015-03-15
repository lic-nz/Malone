using System;
using RestSharp;

namespace LIC.Malone.Core
{
	public class Request
	{
		public string Url { get; set; }
		public string Method { get; set; }
		public string Token { get; set; }

		private Method GetRestSharpMethod()
		{
			switch (Method)
			{
				case "POST":
					return RestSharp.Method.POST;
				default:
					return RestSharp.Method.GET;
			}
		}

		public MaloneRestRequest ToMaloneRestRequest()
		{
			Uri url;
			if (!Uri.TryCreate(Url, UriKind.Absolute, out url))
				throw new Exception(string.Format("Failed to create a URI from the string '{0}'.", Url));
			
			var request = new MaloneRestRequest(url, GetRestSharpMethod());

			request.AddHeader("Accept", "text/xml");
			request.AddHeader("Accept-Encoding", "gzip,deflate");
			request.AddHeader("Authorization", string.Concat("Bearer ", Token));

			return request;
		}
	}
}
