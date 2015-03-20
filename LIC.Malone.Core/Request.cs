using System;
using DotNetOpenAuth.OAuth2;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;

namespace LIC.Malone.Core
{
	public class Request
	{
		public Guid Guid { get; set; }
		public DateTimeOffset At { get; set; }
		public string Url { get; set; }
		public Method Method { get; set; }
		public Response Response { get; set; }

		public NamedAuthorizationState NamedAuthorizationState { get; set; }

		// TODO: Don't pollute core with Caliburn shiz.
		#region Caliburn workarounds

		public string AtString
		{
			get { return At.ToLocalTime().ToString("D"); }
		}

		public string ResponseTime
		{
			get
			{
				var diff = Response.At - this.At;
				return string.Concat((int) diff.TotalMilliseconds, "ms");
			}
		}

		#endregion

		public string BaseUrl
		{
			get { return GetBaseUrl(); }
		}

		public string ResourcePath
		{
			get { return GetResourcePath(); }
		}

		[JsonConstructor]
		public Request()
		{
		}

		public Request(string url) : this(url, Method.GET)
		{
			
		}

		public Request(string url, Method method)
		{
			Guid = Guid.NewGuid();
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

			if (NamedAuthorizationState != null)
				request.AddHeader("Authorization", string.Concat("Bearer ", NamedAuthorizationState.AuthorizationState.AccessToken));

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
