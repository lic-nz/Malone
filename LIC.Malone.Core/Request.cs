using System;
using LIC.Malone.Core.Authentication.OAuth;
using Newtonsoft.Json;
using RestSharp;

namespace LIC.Malone.Core
{
	public class Request
	{
		public const string DateFormatString = "dddd dd MMMM yyyy HH:mm:ss.FFF";

		public Guid Guid { get; set; }
		public DateTimeOffset At { get; set; }
		public string Url { get; set; }
		public Method Method { get; set; }
		public string Body { get; set; }
		public Response Response { get; set; }

		public NamedAuthorizationState NamedAuthorizationState { get; set; }

		// TODO: Don't pollute core with Caliburn shiz.
		#region Caliburn workarounds

		public string AtLocalString
		{
			get { return At.ToLocalTime().ToString(DateFormatString); }
		}

		public string AtUtcString
		{
			get { return string.Concat("UTC: ", At.ToString(DateFormatString)); }
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

			request.AddBody(Body);

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
			var builder = new UriBuilder(url.Scheme, url.Host);

			if (!url.IsDefaultPort)
				builder.Port = url.Port;

			var baseUrl = builder.ToString();

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
