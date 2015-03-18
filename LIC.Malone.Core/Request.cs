using System;
using System.Windows.Media;
using RestSharp;

namespace LIC.Malone.Core
{
	public class Request
	{
		public string Url { get; private set; }
		public Method Method { get; private set; }
		public string Token { get; set; }

		public string BaseUrl
		{
			get { return GetBaseUrl(); }
		}

		public string ResourcePath
		{
			get { return GetResourcePath(); }
		}

		public Request(string url) : this(url, Method.GET)
		{
			
		}

		public Request(string url, Method method)
		{
			Url = url;
			Method = method;
		}

		public MaloneRestRequest ToMaloneRestRequest()
		{
			var url = GetUri();
			var baseUrl = GetBaseUrl(url);
			var resourcePath = GetResourcePath(url);
			
			var request = new MaloneRestRequest(url, baseUrl, resourcePath, Method);

			request.AddHeader("Accept", "text/xml");
			request.AddHeader("Accept-Encoding", "gzip,deflate");
			request.AddHeader("Authorization", string.Concat("Bearer ", Token));

			return request;
		}

		private Uri GetUri()
		{
			Uri url;
			if (!Uri.TryCreate(Url, UriKind.Absolute, out url))
				throw new Exception(string.Format("Failed to create a URI from the string '{0}'.", Url));

			return url;
		}

		private string GetBaseUrl()
		{
			return GetBaseUrl(GetUri());
		}

		private string GetBaseUrl(Uri url)
		{
			var baseUrl = new UriBuilder(url.Scheme, url.Host, url.Port).ToString();

			// Remove trailing slash to keep RestSharp happy.
			return baseUrl.Substring(0, baseUrl.Length - 1);
		}

		private string GetResourcePath()
		{
			return GetResourcePath(GetUri());
		}

		private string GetResourcePath(Uri url)
		{
			var path = url.AbsolutePath;

			// Remove leading slash to keep RestSharp happy.
			if (path.StartsWith("/"))
				path = path.Substring(1);

			return path;
		}
	}
}
