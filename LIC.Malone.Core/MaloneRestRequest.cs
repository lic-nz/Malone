using System;
using RestSharp;

namespace LIC.Malone.Core
{
	public class MaloneRestRequest : RestRequest
	{
		public Uri AbsoluteUrl { get; private set; }
		public string BaseUrl { get; private set; }
		public string ResourceUrl { get; private set; }

		public MaloneRestRequest(Uri url, Method method) : this(url, ToBaseUrl(url), ToResourceUrl(url), method)
		{
		}

		private MaloneRestRequest(Uri absoluteUrl, string baseUrl, string resourceUrl, Method method) : base(resourceUrl, method)
		{
			AbsoluteUrl = absoluteUrl;
			BaseUrl = baseUrl;
			ResourceUrl = resourceUrl;
		}

		private static string ToBaseUrl(Uri url)
		{
			var baseUrl = new UriBuilder(url.Scheme, url.Host, url.Port).ToString();

			// Remove trailing slash to keep RestSharp happy.
			return baseUrl.Substring(0, baseUrl.Length - 1);
		}

		private static string ToResourceUrl(Uri url)
		{
			var path = url.AbsolutePath;

			// Remove leading slash to keep RestSharp happy.
			if (path.StartsWith("/"))
				path = path.Substring(1);

			return path;
		}
	}
}